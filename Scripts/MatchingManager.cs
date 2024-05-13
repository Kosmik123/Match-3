using Bipolar.PuzzleBoard;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        private void SwapManager_OnSwapRequested(Vector2Int pieceCoord1, Vector2Int pieceCoord2)
        {
            Debug.Log($"Swap of {pieceCoord1} and {pieceCoord2} requested");
            if (boardController.IsBusy)
                return;

            SwapPieces(pieceCoord1, pieceCoord2);
            StartCoroutine(TryMatch(
                onSuccess: null, 
                onFail: () => SwapPieces(pieceCoord2, pieceCoord1),
                pieceCoord1, pieceCoord2));
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

        private void SwapPieces(Vector2Int pieceCoord1, Vector2Int pieceCoord2)
        {
            Debug.Log($"Pieces at {pieceCoord1} and {pieceCoord2} swapped");

            boardComponent.SwapPieces(pieceCoord1, pieceCoord2);
            var piece1 = sceneBoard.GetPiece(coords.firstCoord);
            var piece2 = sceneBoard.GetPiece(coords.secondCoord);

            var swapPiecesCommand = new SwapPiecesCommand(piece1, piece2, pieceCoord2, pieceCoord1, piecesSwapManager);
            boardController.RequestCommand(swapPiecesCommand);
        }
        
        private void OnDisable()
        {
            swapRequester.OnSwapRequested -= SwapManager_OnSwapRequested;
        }
    }
}
