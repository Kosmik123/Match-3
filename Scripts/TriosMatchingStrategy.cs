using Bipolar.PuzzleBoard;
using System.Collections.Generic;
using UnityEngine;

namespace Bipolar.Match3
{
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
                DrawDebugLine(leftCoord, rightCoord, color, duration, board);
            }

            foreach (var coord in verticalTrios)
            {
                var downCoord = coord + Vector2Int.up;
                var upCoord = coord + Vector2Int.down;
                DrawDebugLine(upCoord, downCoord, color, duration, board);
            }
        }

        protected void DrawDebugLine(Vector2Int startCoord, Vector2Int endCoord, Color color, float duration, IBoard board)
        {
            Debug.DrawLine(board.CoordToWorld(startCoord), board.CoordToWorld(endCoord), color, duration);
        }
    }

    public class TriosMatchingStrategy : MatchingStrategy<TriosPiecesChain>
    {
        protected override void PopulatePiecesChain(TriosPiecesChain chain, Queue<Vector2Int> coordsQueue, IBoard board)
        {
            FindMatches(board, chain, coordsQueue);
        }

        public static void FindMatches(IBoard board, TriosPiecesChain chain, Queue<Vector2Int> coordsToCheck)
        {
            bool isHexagonal = board.Grid.cellLayout == GridLayout.CellLayout.Hexagon;
            while (coordsToCheck.Count > 0)
            {
                var pieceCoord = coordsToCheck.Dequeue();
                chain.Add(pieceCoord);
                foreach (var direction in GetLinesDirections(board.Grid.cellLayout))
                {
                    TryAddLineToChain(board, chain, pieceCoord, direction, coordsToCheck, isHexagonal);
                }
            }
        }

        public static bool TryAddLineToChain(IBoard board, TriosPiecesChain chain, Vector2Int pieceCoord, Vector2Int direction, Queue<Vector2Int> coordsToCheck, bool isHexagonal)
        {
            var nearCoord = pieceCoord + BoardHelper.GetFixedDirection(pieceCoord, direction, isHexagonal);
            var nearToken = board.GetPiece(nearCoord);
            if (nearToken == null || chain.PieceType != nearToken.Type)
                return false;

            var backCoord = pieceCoord + BoardHelper.GetFixedDirection(pieceCoord, -direction, isHexagonal);
            var backPiece = board.GetPiece(backCoord);
            if (backPiece && chain.PieceType == backPiece.Type)
            {
                chain.IsMatchFound = true;
                TryEnqueueCoord(chain, coordsToCheck, nearCoord);
                TryEnqueueCoord(chain, coordsToCheck, backCoord);
                AddLineToChain(chain, pieceCoord, direction);
                return true;
            }

            var furtherCoord = nearCoord + BoardHelper.GetFixedDirection(nearCoord, direction, isHexagonal);
            var furtherPiece = board.GetPiece(furtherCoord);
            if (furtherPiece && chain.PieceType == furtherPiece.Type)
            {
                chain.IsMatchFound = true;
                TryEnqueueCoord(chain, coordsToCheck, nearCoord);
                TryEnqueueCoord(chain, coordsToCheck, furtherCoord);
                AddLineToChain(chain, nearCoord, direction);
                return true;
            }

            return false;
        }

        public static void AddLineToChain(TriosPiecesChain chain, Vector2Int centerCoord, Vector2Int direction)
        {
            if (direction.x != 0)
                chain.AddHorizontal(centerCoord);
            else if (direction.y != 0)
                chain.AddVertical(centerCoord);
        }

        public static bool TryEnqueueCoord(PiecesChain chain, Queue<Vector2Int> coordsToCheck, Vector2Int coord)
        {
            if (chain.Contains(coord))
                return false;

            if (coordsToCheck.Contains(coord))
                return false;

            coordsToCheck.Enqueue(coord);
            return true;
        }
    }
}
