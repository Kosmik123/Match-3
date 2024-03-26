using Bipolar.PuzzleBoard;
using System.Collections.Generic;
using UnityEngine;

namespace Bipolar.Match3
{
    public class PiecesChain
    {
        public IPieceType PieceType { get; set; }

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

        internal virtual void DrawGizmo(IBoard board)
        {
            foreach (var coord in PiecesCoords)
                Gizmos.DrawSphere(board.CoordToWorld(coord), 0.3f);
        }

        internal virtual void DrawDebug(IBoard board, Color color, float duration)
        { }
    }

    public class TriosPiecesChain : PiecesChain
    {
        private readonly HashSet<Vector2Int> horizontalTrios = new HashSet<Vector2Int>();
        private readonly HashSet<Vector2Int> verticalTrios = new HashSet<Vector2Int>();
        public int HorizontalTriosCount => horizontalTrios.Count;
        public int VerticalTriosCount => verticalTrios.Count;
        
        public void AddHorizontal(Vector2Int lineCenter)
        {
            horizontalTrios.Add(lineCenter);
        }

        public void AddVertical(Vector2Int lineCenter)
        {
            verticalTrios.Add(lineCenter);
        }

        public override void Clear()
        {
            base.Clear();
            verticalTrios.Clear();
            horizontalTrios.Clear();
        }

        public override string ToString() => $"{base.ToString()}, H: {HorizontalTriosCount}, V: {VerticalTriosCount}";

        internal override void DrawDebug(IBoard board, Color color, float duration)
        { 
            base.DrawDebug(board, color, duration);
            foreach (var coord in horizontalTrios)
            {
                var leftCoord = coord + Vector2Int.left;
                var rightCoord = coord + Vector2Int.right;
                DrawLine(leftCoord, rightCoord, color, duration);
            }

            foreach (var coord in verticalTrios)
            {
                var downCoord = coord + Vector2Int.up;
                var upCoord = coord + Vector2Int.down;
                DrawLine(upCoord, downCoord, color, duration);
            }

            void DrawLine(Vector2Int startCoord, Vector2Int endCoord, Color color, float duration)
            {
                Debug.DrawLine(board.CoordToWorld(startCoord), board.CoordToWorld(endCoord), color, duration);
            }
        }
    }
}
