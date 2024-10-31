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

        public override string ToString() => $"{base.ToString()}, {SquaresCount} Sq";

        public override void Clear()
        {
            base.Clear();
            squares.Clear();
        }

        internal override void DrawDebug(IReadOnlySceneBoard board, Color color, float duration)
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
        public override void PopulatePiecesChain(TriosWithSquaresPiecesChain chain, Queue<Vector2Int> coordsQueue, IReadOnlyBoard board)
        {
            bool isHexagonal = board.Layout == GridLayout.CellLayout.Hexagon;
            while (coordsQueue.Count > 0)
            {
                var pieceCoord = coordsQueue.Dequeue();
                chain.Add(pieceCoord);
                foreach (var direction in BoardHelper.GetDirections(board.Layout))
                {
                    TriosMatchingStrategy.TryAddLineToChain(board, chain, pieceCoord, direction, coordsQueue, isHexagonal);
                }

                if (isHexagonal == false) 
                {
                    for (int i = 0; i < BoardHelper.defaultBoardDirections.Length; i++)
                    {
                        TryAddSquareToChain(board, chain, pieceCoord, i);
                    }
                }
            }
        }

        public static bool TryAddSquareToChain(IReadOnlyBoard board, TriosWithSquaresPiecesChain chain, Vector2Int pieceCoord, int directionIndex)
        {
            int xMin = pieceCoord.x;
            int yMin = pieceCoord.y;

            var nextCoord = pieceCoord;
            for (int i = 0; i < 3; i++)
            {
                int coordIndex = (directionIndex + i) % BoardHelper.defaultBoardDirections.Length;
                nextCoord += BoardHelper.defaultBoardDirections[coordIndex];
                
                if (board.ContainsCoord(nextCoord) == false)
                    return false;
                
                var nextPiece = board[nextCoord];
                if (nextPiece == null || chain.PieceColor != nextPiece.Color)
                    return false;

                if (nextCoord.x < xMin)
                    xMin = nextCoord.x;
                if (nextCoord.y < yMin)
                    yMin = nextCoord.y;
            }

            chain.IsMatchFound = true;
            AddSquareToChain(chain, new Vector2Int(xMin, yMin));
            return true;
        }

        public static void AddSquareToChain(TriosWithSquaresPiecesChain chain, Vector2Int bottomLeftCoord)
        {
            chain.AddSquare(bottomLeftCoord);
            chain.Add(bottomLeftCoord);
            chain.Add(bottomLeftCoord + Vector2Int.right);
            chain.Add(bottomLeftCoord + Vector2Int.up);
            chain.Add(bottomLeftCoord + Vector2Int.one);
        }
    }

}
