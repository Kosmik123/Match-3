using Bipolar.PuzzleBoard;
using System.Collections.Generic;
using UnityEngine;

namespace Bipolar.Match3
{
    public class PiecesClearManager : MonoBehaviour
    {
        public event System.Action OnAllPiecesCleared;

        [SerializeField]
        private BoardController boardController;

        private readonly List<Piece> currentlyClearedPieces = new List<Piece>();
        public int CurrentlyClearedPiecesCount => currentlyClearedPieces.Count;

        protected virtual void Reset()
        {
            boardController = FindObjectOfType<BoardController>();
        }

        public void ClearChainPieces(PiecesChain chain)
        {
            foreach (var coord in chain.PiecesCoords)
            {
                var piece = boardController.Board.GetPiece(coord);

                currentlyClearedPieces.Add(piece);
                boardController.Pieces[coord] = null;
            }

            foreach (var piece in currentlyClearedPieces)
            {
                piece.OnCleared += Piece_OnCleared;
                if (piece.TryGetComponent<PieceClearingBehavior>(out var pieceClearing))
                {
                    pieceClearing.Clear();
                }
                else
                {
                    piece.IsCleared = true;
                }
            }
        }

        private void Piece_OnCleared(Piece piece)
        {
            piece.OnCleared -= Piece_OnCleared;
            currentlyClearedPieces.Remove(piece);
            if (currentlyClearedPieces.Count <= 0)
                OnAllPiecesCleared?.Invoke();
        }
    }
}
