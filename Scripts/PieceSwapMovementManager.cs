using Bipolar.PuzzleBoard.Components;
using UnityEngine;

namespace Bipolar.Match3
{
    public class PieceSwapMovementManager : MonoBehaviour
    {
        [SerializeField]
        private MatchController matchController;
        [SerializeField]
        private BoardComponent boardComponent;

        protected virtual void Reset()
        {
            boardComponent = FindObjectOfType<BoardComponent>();
            matchController = FindObjectOfType<MatchController>();
        }

        private void OnEnable()
        {
            matchController.OnPiecesSwapped += MatchController_OnPiecesSwapped;
        }

        private void MatchController_OnPiecesSwapped(Vector2Int pieceCoord1, Vector2Int pieceCoord2)
        {
            MovePiece(pieceCoord1);
            MovePiece(pieceCoord2);
        }

        private void MovePiece(Vector2Int coord)
        {
            var piece = boardComponent.GetPieceComponent(coord);
            piece.MoveTo(boardComponent.CoordToWorld(coord));
        }

        private void OnDisable()
        {
            matchController.OnPiecesSwapped -= MatchController_OnPiecesSwapped;
        }
    }
}
