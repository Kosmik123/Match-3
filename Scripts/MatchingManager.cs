using Bipolar.PuzzleBoard.Components;
using UnityEngine;

namespace Bipolar.Match3
{
    public class MatchingManager : MonoBehaviour
    {
        [SerializeField]
        private BoardController boardController;
        [SerializeField]
        private SwapManager swapManager;
        [SerializeField]
        private MatchController matchController;
        [SerializeField]
        private PiecesClearManager piecesClearManager;
        
        [SerializeField]
        private int combo;
        public int Combo => combo;

        [SerializeField]
        private MatchPredictor testMatchPredictor;

        protected virtual void Reset()
        {
            boardController = FindObjectOfType<BoardController>();
            swapManager = FindObjectOfType<SwapManager>();
            matchController = FindObjectOfType<MatchController>();
            piecesClearManager = FindObjectOfType<PiecesClearManager>();
        }

        private void OnEnable()
        {
            boardController.OnPiecesColapsed += BoardController_OnPiecesColapsed;
            swapManager.OnSwapRequested += SwapManager_OnSwapRequested;
            piecesClearManager.OnAllPiecesCleared += PiecesClearManager_OnAllPiecesCleared;
            matchController.OnPiecesMatched += MatchController_OnPiecesMatched;
        }

        private void MatchController_OnPiecesMatched(PiecesChain chain)
        {
            combo++;
            piecesClearManager.ClearPiecesInChain(chain);
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
            
            if (piecesClearManager.CurrentlyClearedPiecesCount > 0)
                return;

            combo = 0;
            matchController.StartSwappingPieces(pieceCoord1, pieceCoord2);
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

        private void OnDisable()
        {
            boardController.OnPiecesColapsed -= BoardController_OnPiecesColapsed;
            swapManager.OnSwapRequested -= SwapManager_OnSwapRequested;
            piecesClearManager.OnAllPiecesCleared -= PiecesClearManager_OnAllPiecesCleared;
            matchController.OnPiecesMatched -= MatchController_OnPiecesMatched;
        }
    }
}
