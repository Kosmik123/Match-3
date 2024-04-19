using Bipolar.PuzzleBoard;
using Bipolar.PuzzleBoard.Components;
using System.Collections.Generic;
using UnityEngine;

namespace Bipolar.Match3
{
    public class PiecesChain
    {
        public IPieceColor PieceType { get; set; }

        protected readonly HashSet<Vector2Int> piecesCoords = new HashSet<Vector2Int>();
        public IReadOnlyCollection<Vector2Int> PiecesCoords => piecesCoords;
        public bool IsMatchFound { get; set; } = false;
        public bool Contains(Vector2Int pieceCoord) => piecesCoords.Contains(pieceCoord);
        public int Size => piecesCoords.Count;

        public void Add(Vector2Int pieceCoord)
        {
            piecesCoords.Add(pieceCoord);
        }

        public virtual void Clear()
        {
            PieceType = null;
            piecesCoords.Clear();
        }

        public override string ToString() => $"Pieces Chain ({(PieceType as Object).name}): {Size}";

        internal virtual void DrawGizmo(IReadOnlyBoardComponent board)
        {
            foreach (var coord in PiecesCoords)
                Gizmos.DrawSphere(board.CoordToWorld(coord), 0.3f);
        }

        internal virtual void DrawDebug(IReadOnlyBoardComponent board, Color color, float duration)
        { }
    }
}
