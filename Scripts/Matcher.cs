using Bipolar.PuzzleBoard;
using Bipolar.PuzzleBoard.Rectangular;
using System.Collections.Generic;
using UnityEngine;

namespace Bipolar.Match3
{
    public abstract class Matcher : MonoBehaviour
    {
        public abstract void FindAndCreatePieceChains();
        public abstract IReadOnlyList<PiecesChain> PieceChains { get; }
    }

    public abstract class Matcher<TBoard> : Matcher
        where TBoard : class, IBoard
    {
        private static readonly Vector2Int[] defaultChainsDirections =
        {
            Vector2Int.right,
            Vector2Int.up,
            Vector2Int.left,
            Vector2Int.down
        };

        private static readonly Vector2Int[] hexagonalChainsDirections =
        {
            Vector2Int.up + Vector2Int.right,
            Vector2Int.right,
            Vector2Int.up + Vector2Int.left,
            Vector2Int.down + Vector2Int.right,
            Vector2Int.left,
            Vector2Int.down + Vector2Int.left,
        };

        private TBoard _board;
        public TBoard Board
        {
            get
            {
                if (_board == null) 
                    _board = GetComponent<TBoard>(); 
                return _board;
            }
        }


        protected readonly List<PiecesChain> pieceChains = new List<PiecesChain>();
        public override IReadOnlyList<PiecesChain> PieceChains => pieceChains;

        protected void CreatePiecesChain(Vector2Int coord, Queue<Vector2Int> coordsToCheck = null)
        {
            coordsToCheck ??= new Queue<Vector2Int>();
            coordsToCheck.Clear();
            coordsToCheck.Enqueue(coord);
            var chain = new TriosPiecesChain();
            chain.PieceType = Board.GetPiece(coord).Type;
            FindMatches(Board, chain, coordsToCheck);

            if (chain.IsMatchFound)
                pieceChains.Add(chain);
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

        public static IReadOnlyList<Vector2Int> GetLinesDirections(GridLayout.CellLayout layout) => layout == GridLayout.CellLayout.Hexagon
            ? (IReadOnlyList<Vector2Int>)hexagonalChainsDirections
            : defaultChainsDirections;

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

        private void OnDrawGizmos()
        {
            if (Board != null)
            {
                var random = new System.Random(PieceChains.Count);
                foreach (var chain in PieceChains)
                {
                    random.Next();
                    var color = Color.HSVToRGB((float)random.NextDouble(), 1, 1);
                    color.a = 0.5f;
                    Gizmos.color = color;
                    chain.DrawGizmo(Board);
                }
            }
        }
    }
}
