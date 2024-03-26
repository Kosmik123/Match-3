using Bipolar.PuzzleBoard;
using System.Collections.Generic;
using UnityEngine;

namespace Bipolar.Match3
{
    public abstract class MatchingStrategy : ScriptableObject
    {
        internal static readonly Vector2Int[] defaultChainsDirections =
        {
            Vector2Int.right,
            Vector2Int.up,
            Vector2Int.left,
            Vector2Int.down
        };

        internal static readonly Vector2Int[] hexagonalChainsDirections =
        {
            Vector2Int.up + Vector2Int.right,
            Vector2Int.right,
            Vector2Int.up + Vector2Int.left,
            Vector2Int.down + Vector2Int.right,
            Vector2Int.left,
            Vector2Int.down + Vector2Int.left,
        };

        public static IReadOnlyList<Vector2Int> GetLinesDirections(GridLayout.CellLayout layout) => layout == GridLayout.CellLayout.Hexagon
            ? (IReadOnlyList<Vector2Int>)hexagonalChainsDirections
            : defaultChainsDirections;

        public PiecesChain GetPiecesChain(Queue<Vector2Int> coordsQueue, IBoard board)
        {
            return CreatePiecesChain(board.GetPiece(coordsQueue.Peek()).Type, coordsQueue, board);
        }

        protected abstract PiecesChain CreatePiecesChain(IPieceType pieceType, Queue<Vector2Int> coordsQueue, IBoard board);
    }

    public abstract class MatchingStrategy<T> : MatchingStrategy
        where T : PiecesChain, new()
    {
        protected sealed override PiecesChain CreatePiecesChain(IPieceType pieceType, Queue<Vector2Int> coordsQueue, IBoard board)
        {
            var chain = new T();
            chain.PieceType = pieceType;
            PopulatePiecesChain(chain, coordsQueue, board);
            return chain;
        }

        protected abstract void PopulatePiecesChain(T chain, Queue<Vector2Int> coordsQueue, IBoard board);
    }
}
