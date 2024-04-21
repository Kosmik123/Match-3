using Bipolar.PuzzleBoard.Components;
using UnityEngine;

namespace Bipolar.Match3
{
    public class PieceSwapMovementManager : MonoBehaviour
    {
        [SerializeField]
        private MatchingManager matchingManager;
        [SerializeField]
        private BoardController boardController;

        protected virtual void Reset()
        {
            boardController = FindObjectOfType<BoardController>();
            matchingManager = FindObjectOfType<MatchingManager>();
        }

        private void OnEnable()
        {
            matchingManager.OnPiecesSwapped += MatchController_OnPiecesSwapped;
        }

        private void MatchController_OnPiecesSwapped(Vector2Int pieceCoord1, Vector2Int pieceCoord2)
        {
            MovePiece(pieceCoord1);
            MovePiece(pieceCoord2);
        }

        private void MovePiece(Vector2Int coord)
        {
            var piece = boardController.BoardComponent.GetPieceComponent(coord);
            piece.MoveTo(boardController.BoardComponent.CoordToWorld(coord));
        }

        private void OnDisable()
        {
            matchingManager.OnPiecesSwapped -= MatchController_OnPiecesSwapped;
        }
    }
}
