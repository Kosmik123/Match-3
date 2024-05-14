using Bipolar.PuzzleBoard;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bipolar.Match3
{

    public class MatchingManager : MonoBehaviour
    {
        [SerializeField]
        private BoardController boardController;
        [SerializeField]
        private SwapRequester swapRequester;
        [SerializeField]
        private MatchController matchController;
        [SerializeField]
        private PiecesClearManager piecesClearManager;
        [SerializeField]
        private BoardCollapseController boardCollapseController;
        [SerializeField]
        private SceneBoard sceneBoard;
        [SerializeField]
        private PiecesSwapManager piecesSwapManager;

        [SerializeField]
        private int combo;
        public int Combo => combo;

        [SerializeField]
        private MatchPredictor testMatchPredictor;

        protected virtual void Reset()
        {
            boardController = FindObjectOfType<BoardController>();
            swapRequester = FindObjectOfType<SwapRequester>();
            matchController = FindObjectOfType<MatchController>();
            piecesClearManager = FindObjectOfType<PiecesClearManager>();
        }

        private void OnEnable()
        {
            swapRequester.OnSwapRequested += SwapManager_OnSwapRequested;
        }

        private IEnumerator Start()
        {
            boardCollapseController.Collapse();
            yield return TryMatch();
        }

        private void SwapManager_OnSwapRequested(CoordsPair coords)
        {
            Debug.Log($"Swap of {coords.firstCoord} and {coords.secondCoord} requested");
            if (boardController.IsBusy)
                return;

            SwapPieces(coords);
            StartCoroutine(TryMatch(
                onSuccess: testMatchPredictor.FindPossibleChains,
                onFail: () => SwapPieces(coords),
                coords.firstCoord, coords.secondCoord));
        }

        private IEnumerator TryMatch(System.Action onSuccess = null, System.Action onFail = null, params Vector2Int[] coords)
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
                ClearChains(matchController.Chains);
                boardCollapseController.Collapse();
                yield return null;
            }
            while (true);
            var action = success ? onSuccess : onFail;
            action?.Invoke();
        }

        // to powinno znaleźć się w osobnej klasie która zarządza tworzeniem bomb w rzędach od długości 4+
        private void ClearChains(IReadOnlyList<PiecesChain> chains)
        {
            var piecesToClear = new List<Piece>();
            foreach (var chain in chains)
            {
                foreach (var coord in chain.PiecesCoords)
                {
                    piecesToClear.Add(sceneBoard.GetPiece(coord));
                }
            }

            ClearPieces(piecesToClear);
        }

        private void ClearPieces(IReadOnlyList<Piece> pieces)
        {
            foreach (var piece in pieces)
                piece.ClearPiece();

            var command = new ClearPiecesCommand(pieces, piecesClearManager);
            boardController.RequestCommand(command);
        }

        private void SwapPieces(CoordsPair coords)
        {
            Debug.Log($"Pieces at {coords.firstCoord} and {coords.secondCoord} swapped");

            var piece1 = sceneBoard.GetPiece(coords.firstCoord);
            var piece2 = sceneBoard.GetPiece(coords.secondCoord);

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
