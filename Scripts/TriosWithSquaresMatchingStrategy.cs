using Bipolar.PuzzleBoard;
using System.Collections.Generic;
using UnityEngine;

namespace Bipolar.Match3
{
    public class TriosWithSquaresPiecesChain : TriosPiecesChain
    {
        private readonly HashSet<Vector2Int> squares = new HashSet<Vector2Int>();
        public int SquaresCount => squares.Count;

        public void AddSquare(Vector2Int squareBottomLeft)
        {
            squares.Add(squareBottomLeft);
        }

        public override string ToString() => $"{base.ToString()}, Sq: {SquaresCount}";

        public override void Clear()
        {
            base.Clear();
            squares.Clear();
        }

        internal override void DrawDebug(IBoard board, Color color, float duration)
        {
            base.DrawDebug(board, color, duration);
            foreach (var bottomLeft in squares)
            {
                var topLeft = bottomLeft + Vector2Int.up;
                var bottomRight = bottomLeft + Vector2Int.right;
                var topRight = bottomLeft + Vector2Int.one;
                DrawDebugLine(bottomLeft, bottomRight, color, duration, board);
                DrawDebugLine(bottomLeft, topLeft, color, duration, board);
                DrawDebugLine(bottomRight, topRight, color, duration, board);
                DrawDebugLine(topLeft, topRight, color, duration, board);
            }
        }
    }

    public class TriosWithSquaresMatchingStrategy : MatchingStrategy<TriosWithSquaresPiecesChain>
    {
        protected override void PopulatePiecesChain(TriosWithSquaresPiecesChain chain, Queue<Vector2Int> coordsQueue, IBoard board)
        {

        }
    }
}
