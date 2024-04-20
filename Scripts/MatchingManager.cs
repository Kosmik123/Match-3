using Bipolar.PuzzleBoard.Components;
using UnityEngine;

namespace Bipolar.Match3
{
    public class MatchingManager : MonoBehaviour
    {
        public event PiecesSwapEventHandler OnPiecesSwapped;

        [SerializeField]
        private BoardController boardController;
        [SerializeField]
        private SwapRequester swapRequester;
        [SerializeField]
        private MatchController matchController;
        
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
            //piecesClearManager = FindObjectOfType<PiecesClearManager>();
        }

        private void OnEnable()
        {
            boardController.OnPiecesColapsed += BoardController_OnPiecesColapsed;
            swapRequester.OnSwapRequested += SwapManager_OnSwapRequested;
            //piecesClearManager.OnAllPiecesCleared += PiecesClearManager_OnAllPiecesCleared;
            //matchController.OnPiecesMatched += MatchController_OnPiecesMatched;
        }

        private void MatchController_OnPiecesMatched(PiecesChain chain)
        {
            combo++;
            ClearPiecesInChain(chain);
        }

        private void Start()
        {
            boardController.Collapse();
        }

        private void SwapManager_OnSwapRequested(Vector2Int pieceCoord1, Vector2Int pieceCoord2)
        {
            Debug.Log($"Swap of {pieceCoord1} and {pieceCoord2} requested");
            if (boardController.ArePiecesMoving || boardController.IsCollapsing)
                return;

            //if (piecesClearManager.CurrentlyClearedPiecesCount > 0)
            //    return;

            TrySwapAndMatch(pieceCoord1, pieceCoord2);
        }

        private bool TrySwapAndMatch(Vector2Int pieceCoord1, Vector2Int pieceCoord2)
        {
            combo = 0;
            SwapPieces(pieceCoord1, pieceCoord2);
            bool shouldSwapBack = true;
            do 
            {
                combo++;
                bool matched = matchController.FindMatches();
                if (matched == false)
                    break;

                shouldSwapBack = false;
                foreach (var chain in matchController.Chains)
                    ClearPiecesInChain(chain);

                boardController.Collapse();
            }
            while (true);

            if (shouldSwapBack)
                SwapPieces(pieceCoord2, pieceCoord1);
            
            return shouldSwapBack;
        }

        private void ClearPiecesInChain(PiecesChain chain)
        {
            foreach (var coord in chain.PiecesCoords)
            {
                var piece = boardController.BoardComponent.GetPiece(coord);
                piece.Clear();
            }
        }

        private void BoardController_OnPiecesColapsed()
        {
            matchController.FindMatches();
        }

        private void PiecesClearManager_OnAllPiecesCleared()
        {
            boardController.Collapse();
            testMatchPredictor?.FindPossibleChains();
        }

        private void SwapPieces(Vector2Int pieceCoord1, Vector2Int pieceCoord2)
        {
            Debug.Log($"Pieces at {pieceCoord1} and {pieceCoord2} swapped");
            boardController.BoardComponent.SwapPieces(pieceCoord1, pieceCoord2);
            OnPiecesSwapped?.Invoke(pieceCoord1, pieceCoord2);
        }
        
        private void OnDisable()
        {
            boardController.OnPiecesColapsed -= BoardController_OnPiecesColapsed;
            swapRequester.OnSwapRequested -= SwapManager_OnSwapRequested;
            //piecesClearManager.OnAllPiecesCleared -= PiecesClearManager_OnAllPiecesCleared;
            matchController.OnPiecesMatched -= MatchController_OnPiecesMatched;
        }
    }
}
