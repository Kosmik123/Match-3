using Bipolar.PuzzleBoard;
using System.Collections.Generic;
using UnityEngine;

namespace Bipolar.Match3
{
    public abstract class MatchingStrategy : ScriptableObject
    {
        public static readonly Vector2Int[] defaultChainsDirections =
        {
            Vector2Int.right,
            Vector2Int.up,
            Vector2Int.left,
            Vector2Int.down
        };

        public static readonly Vector2Int[] hexagonalChainsDirections =
        {
            Vector2Int.right,
            Vector2Int.up + Vector2Int.right,
            Vector2Int.up + Vector2Int.left,
            Vector2Int.left,
            Vector2Int.down + Vector2Int.left,
            Vector2Int.down + Vector2Int.right,
        };

        protected abstract PiecesChain CreatePiecesChain(IPieceColor pieceType, Queue<Vector2Int> coordsToCheck, IBoard board);

        public static IReadOnlyList<Vector2Int> GetLinesDirections(GridLayout.CellLayout layout) => layout == GridLayout.CellLayout.Hexagon
            ? (IReadOnlyList<Vector2Int>)hexagonalChainsDirections
            : defaultChainsDirections;

        public PiecesChain GetPiecesChain(Queue<Vector2Int> coordsQueue, IBoard board)
        {
            return CreatePiecesChain(board[coordsQueue.Peek()].Color, coordsQueue, board);
        }

        public static bool TryEnqueueCoord(PiecesChain chain, Queue<Vector2Int> coordsQueue, Vector2Int coord)
        {
            if (chain.Contains(coord))
                return false;

            if (coordsQueue.Contains(coord))
                return false;

            coordsQueue.Enqueue(coord);
            return true;
        }
    }

    public abstract class MatchingStrategy<T> : MatchingStrategy
        where T : PiecesChain, new()
    {
        protected sealed override PiecesChain CreatePiecesChain(IPieceColor pieceType, Queue<Vector2Int> coordsToCheck, IBoard board)
        {
            var chain = new T();
            chain.PieceType = pieceType;
            PopulatePiecesChain(chain, coordsToCheck, board);
            return chain;
        }

        public abstract void PopulatePiecesChain(T chain, Queue<Vector2Int> coordsToCheck, IBoard board);
    }
}
