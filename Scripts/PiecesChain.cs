using Bipolar.PuzzleBoard;
using System.Collections.Generic;
using UnityEngine;

namespace Bipolar.Match3
{
    public class PiecesChain
    {
        public Vector2Int StartingCoord { get; set; }
        public IPieceColor PieceColor { get; set; }
        
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
            PieceColor = null;
            piecesCoords.Clear();
        }

        public override string ToString() => $"Chain {(PieceColor as Object).name}: {Size} Pieces";

        internal virtual void DrawGizmo(IReadOnlySceneBoard board)
        {
            foreach (var coord in PiecesCoords)
                Gizmos.DrawSphere(board.CoordToWorld(coord), 0.3f);
        }

        internal virtual void DrawDebug(IReadOnlySceneBoard board, Color color, float duration)
        { }
    }
}
