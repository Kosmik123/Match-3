using Bipolar.PuzzleBoard;
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
        private BoardComponent boardComponent;
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

        private void Start()
        {
            do
            {
                boardCollapseController.Collapse();
                bool matchesAtStart = matchController.FindMatches();
                if (matchesAtStart == false)
                    return;

                ClearChains(matchController.Chains);
            }
            while (true);
        }

        private void SwapManager_OnSwapRequested(Vector2Int pieceCoord1, Vector2Int pieceCoord2)
        {
            Debug.Log($"Swap of {pieceCoord1} and {pieceCoord2} requested");
            if (boardController.IsBusy)
                return;

            if (TrySwapAndMatch(pieceCoord1, pieceCoord2) == false)
            {
                SwapPieces(pieceCoord2, pieceCoord1);
            }
        }

        private bool TrySwapAndMatch(Vector2Int pieceCoord1, Vector2Int pieceCoord2)
        {
            combo = 0;
            SwapPieces(pieceCoord1, pieceCoord2);
            bool success = false;
            do 
            {
                combo++;
                bool matched = matchController.FindMatches();
                if (matched == false)
                    break;

                success = true;
                ClearChains(matchController.Chains);

                boardCollapseController.Collapse();
            }
            while (true);
            return success;
        }

        private void ClearChains(IReadOnlyList<PiecesChain> chains)
        {
            var piecesToClear = new List<Piece>(); 
            foreach (var chain in chains)
            {
                foreach (var coord in chain.PiecesCoords)
                {
                    piecesToClear.Add(boardComponent.GetPiece(coord));
                }
            }

            ClearPieces(piecesToClear);
        }

        private void ClearPieces(List<Piece> pieces)
        {
            foreach (var piece in pieces)
                piece.Clear();

            var command = new ClearPiecesCommand(pieces, piecesClearManager);
            boardController.RequestCommand(command);
        }

        private void PiecesClearManager_OnAllPiecesCleared()
        {
            boardCollapseController.Collapse();
            testMatchPredictor?.FindPossibleChains();
        }

        private void SwapPieces(Vector2Int pieceCoord1, Vector2Int pieceCoord2)
        {
            Debug.Log($"Pieces at {pieceCoord1} and {pieceCoord2} swapped");

            var piece1 = boardComponent.GetPiece(pieceCoord1);
            var piece2 = boardComponent.GetPiece(pieceCoord2);
            
            boardComponent.SwapPieces(pieceCoord1, pieceCoord2);

            var swapPiecesCommand = new SwapPiecesCommand(piece1, piece2, pieceCoord2, pieceCoord1, piecesSwapManager);
            boardController.RequestCommand(swapPiecesCommand);
        }
        
        private void OnDisable()
        {
            swapRequester.OnSwapRequested -= SwapManager_OnSwapRequested;
        }
    }
}
