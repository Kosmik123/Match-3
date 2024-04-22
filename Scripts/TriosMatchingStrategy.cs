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

        public override string ToString() => $"{base.ToString()}, {HorizontalTriosCount} H, {VerticalTriosCount} V";

        internal override void DrawDebug(IReadOnlyBoardComponent board, Color color, float duration)
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

        protected void DrawDebugLine(Vector2Int startCoord, Vector2Int endCoord, Color color, float duration, IReadOnlyBoardComponent board)
        {
            Debug.DrawLine(board.CoordToWorld(startCoord), board.CoordToWorld(endCoord), color, duration);
        }
    }

    public class TriosMatchingStrategy : MatchingStrategy<TriosPiecesChain>
    {
        public override void PopulatePiecesChain(TriosPiecesChain chain, Queue<Vector2Int> coordsToCheck, IReadOnlyBoard board)
        {
            bool isHexagonal = board.Layout == GridLayout.CellLayout.Hexagon;
            while (coordsToCheck.Count > 0)
            {
                var pieceCoord = coordsToCheck.Dequeue();
                chain.Add(pieceCoord);
                foreach (var direction in BoardHelper.GetDirections(board.Layout))
                {
                    TryAddLineToChain(board, chain, pieceCoord, direction, coordsToCheck, isHexagonal);
                }
            }
        }

        public static bool TryAddLineToChain(IReadOnlyBoard boardData, TriosPiecesChain chain, Vector2Int pieceCoord, Vector2Int direction, Queue<Vector2Int> coordsToCheck, bool isHexagonal)
        {
            var nearCoord = pieceCoord + BoardHelper.GetCorrectedDirection(pieceCoord, direction, isHexagonal);

            if (boardData.ContainsCoord(nearCoord) == false) 
                return false;

            var nearPiece = boardData[nearCoord];
            if (nearPiece == null || chain.PieceType != nearPiece.Color)
                return false;

            var backCoord = pieceCoord + BoardHelper.GetCorrectedDirection(pieceCoord, -direction, isHexagonal);
            if (boardData.ContainsCoord(backCoord) == false)
                return false;
            
            var backPiece = boardData[backCoord];
            if (backPiece != null && (chain.PieceType == backPiece.Color))
            {
                chain.IsMatchFound = true;
                TryEnqueueCoord(chain, coordsToCheck, nearCoord);
                TryEnqueueCoord(chain, coordsToCheck, backCoord);
                AddLineToChain(chain, pieceCoord, direction);
                return true;
            }

            var furtherCoord = nearCoord + BoardHelper.GetCorrectedDirection(nearCoord, direction, isHexagonal);
            if (boardData.ContainsCoord(furtherCoord) == false)
                return false;
            
            var furtherPiece = boardData[furtherCoord];
            if (furtherPiece != null && (chain.PieceType == furtherPiece.Color))
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
    }
}
