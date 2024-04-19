using Bipolar.PuzzleBoard;
using Bipolar.PuzzleBoard.Components;
using System.Collections.Generic;
using UnityEngine;

namespace Bipolar.Match3
{
    public class PiecesClearManager : MonoBehaviour
    {
        public event System.Action OnAllPiecesCleared;

        [SerializeField]
        private BoardController boardController;

        private readonly List<PieceComponent> currentlyClearedPieces = new List<PieceComponent>();
        public int CurrentlyClearedPiecesCount => currentlyClearedPieces.Count;

        protected virtual void Reset()
        {
            boardController = FindObjectOfType<BoardController>();
        }

        public void ClearChainPieces(PiecesChain chain)
        {
            foreach (var coord in chain.PiecesCoords)
            {
                var piece = boardController.BoardComponent.GetPiece(coord);
                
                currentlyClearedPieces.Add(piece);
                boardController.BoardComponent.Board[coord] = null;
            }

            foreach (var piece in currentlyClearedPieces)
            {
                if (piece == null) 
                    Debug.LogError("null in chain");

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

        private void Piece_OnCleared(PieceComponent piece)
        {
            piece.OnCleared -= Piece_OnCleared;
            currentlyClearedPieces.Remove(piece);
            if (currentlyClearedPieces.Count <= 0)
                OnAllPiecesCleared?.Invoke();
        }
    }
}
