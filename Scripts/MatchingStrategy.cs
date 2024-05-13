using Bipolar.PuzzleBoard;
using System.Collections.Generic;
using UnityEngine;

namespace Bipolar.Match3
{
    public interface IMatchingStrategy
    {
        PiecesChain GetPiecesChain(Vector2Int startingCoord, IReadOnlyBoard board, Queue<Vector2Int> coordsQueue = null);
    }

    public abstract class MatchingStrategy : ScriptableObject, IMatchingStrategy
    {
        protected abstract PiecesChain CreatePiecesChain(Vector2Int startingCoord, IReadOnlyBoard board, Queue<Vector2Int> coordsToCheck = null);

        public PiecesChain GetPiecesChain(Vector2Int startingCoord, IReadOnlyBoard board, Queue<Vector2Int> coordsQueue = null)
        {
            return CreatePiecesChain(startingCoord, board, coordsQueue);
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
        protected sealed override PiecesChain CreatePiecesChain(Vector2Int startingPieceCoord, IReadOnlyBoard board, Queue<Vector2Int> coordsToCheck = null)
        {
            coordsToCheck ??= new Queue<Vector2Int>();
            var chain = CreatePiecesChain(startingPieceCoord, board[startingPieceCoord].Color);
            coordsToCheck.Enqueue(startingPieceCoord);
            PopulatePiecesChain(chain, coordsToCheck, board);
            return chain;
        }

        private static T CreatePiecesChain(Vector2Int startingCoord, IPieceColor pieceColor)
        {
            var chain = new T();
            chain.PieceColor = pieceColor;
            chain.StartingCoord = startingCoord;
            return chain;
        }

        public abstract void PopulatePiecesChain(T chain, Queue<Vector2Int> coordsToCheck, IReadOnlyBoard board);
    }
}
