using Bipolar.PuzzleBoard;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bipolar.Match3
{
    public abstract class ChainsProcessor : MonoBehaviour
    {
        public abstract void ProcessChains(IEnumerable<PiecesChain> chains);
    }

    public class MatchingManager : MonoBehaviour
    {
        [SerializeField]
        private BoardController boardController;
        [SerializeField]
        private SwapRequester swapRequester;
        [SerializeField]
        private MatchController matchController;
        [SerializeField]
        private BoardCollapseController boardCollapseController;
        [SerializeField]
        private SceneBoard sceneBoard;
        [SerializeField]
        private PiecesSwapManager piecesSwapManager;
        [SerializeField]
        private BoardShufflerWrapper shufflerWrapper;

        [SerializeField]
        private int combo;
        public int Combo => combo;

        [SerializeField]
        private MatchPredictor testMatchPredictor;

        [SerializeField]
        private ChainsProcessor[] piecesChainProcessors; 

        protected virtual void Reset()
        {
            boardController = FindObjectOfType<BoardController>();
            swapRequester = FindObjectOfType<SwapRequester>();
            matchController = FindObjectOfType<MatchController>();
        }

        private void OnEnable()
        {
            swapRequester.OnSwapRequested += SwapManager_OnSwapRequested;
        }

        private void Start()
        {
            boardCollapseController.Collapse();
            MatchPieces();
        }

        private void SwapManager_OnSwapRequested(CoordsPair coords)
        {
            Debug.Log($"Swap of {coords.firstCoord} and {coords.secondCoord} requested");
            if (boardController.IsBusy)
                return;

            TrySwap(coords);
        }

        private void TrySwap(CoordsPair coords)
        {
            SwapPieces(coords);
            MatchPieces(
                onFail: () => SwapPieces(coords),
                coords.firstCoord, coords.secondCoord);
        }

        private void FindPossibleMatches()
        {
            var matches = new Dictionary<CoordsPair, List<PiecesChain>>();
            testMatchPredictor.FindPossibleChains(matches);
            if (matches.Count == 0)
            {
                ShuffleBoard();
            }
        }

        private void ShuffleBoard()
        {
            Debug.LogWarning("No matches possible!");
            shufflerWrapper.ShufflePieces();
            MatchPieces(FindPossibleMatches);
        }

        public readonly struct SwapSignalCommand : IBoardCommand
        {
            private readonly CoordsPair coordsPair;
            private readonly SwapRequester swapRequester;

            public SwapSignalCommand(CoordsPair coordsPair, SwapRequester swapRequester)
            {
                if (coordsPair.firstCoord == coordsPair.secondCoord)
                {
                    Debug.LogError("!!!");
                }
                this.coordsPair = coordsPair;
                this.swapRequester = swapRequester;
            }

            public IEnumerator Execute()
            {
                swapRequester.RequestSwap(coordsPair);
                yield return null;
            }

            public override string ToString()
            {
                return $"Request to swap {coordsPair.firstCoord} and {coordsPair.secondCoord}";
            }
        }

        private void MatchPieces(System.Action onFail = null, params Vector2Int[] coords)
        {
            StartCoroutine(MatchPiecesCo(FindPossibleMatches, onFail, coords));
        }

        private IEnumerator MatchPiecesCo(System.Action onSuccess = null, System.Action onFail = null, params Vector2Int[] coords)
        {
            combo = 0;
            bool success = false;
            do
            {
                combo++;
                bool matched = matchController.FindMatches(coords);
                if (matched == false)
                    break;

                success = true;
                ProcessChains(matchController.Chains);
                boardCollapseController.Collapse();
                yield return null;
            }
            while (true);
            var action = success ? onSuccess : onFail;
            action?.Invoke();
        }

        // to powinno znaleźć się w osobnej klasie która zarządza tworzeniem bomb w rzędach od długości 4+

        // Tak, to będzie oddzielny behavior do ustawienia w inspektorze,
        // który ustala w jakis sposób i co się dzieje przy niszczeniu pionków
        // nazywa się on ChainProcessor
        private void ProcessChains(IReadOnlyList<PiecesChain> chains)
        {
            foreach (var processor in piecesChainProcessors)
                processor.ProcessChains(chains);
        }

        private void SwapPieces(CoordsPair coords)
        {
            Debug.Log($"Pieces at {coords.firstCoord} and {coords.secondCoord} swapped");

            var piece1 = sceneBoard.Board[coords.firstCoord];
            var piece2 = sceneBoard.Board[coords.secondCoord];

            sceneBoard.SwapPieces(coords);

            var swapPiecesCommand = new SwapPiecesCommand(piece1, piece2, coords.secondCoord, coords.firstCoord, piecesSwapManager);
            boardController.RequestCommand(swapPiecesCommand);
        }

        private void OnDisable()
        {
            swapRequester.OnSwapRequested -= SwapManager_OnSwapRequested;
        }
    }
}
