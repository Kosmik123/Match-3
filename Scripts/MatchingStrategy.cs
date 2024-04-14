using Bipolar.PuzzleBoard;
using System.Collections.Generic;
using UnityEngine;

namespace Bipolar.Match3
{
    public interface IMatchingStrategy
    {
        PiecesChain GetPiecesChain(Queue<Vector2Int> coordsQueue, IBoard board);
    }

    public abstract class MatchingStrategy : ScriptableObject, IMatchingStrategy
    {
        protected abstract PiecesChain CreatePiecesChain(IPieceColor pieceType, Queue<Vector2Int> coordsToCheck, IBoard board);

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
            var chain = CreatePiecesChain(pieceType); 
            PopulatePiecesChain(chain, coordsToCheck, board);
            return chain;
        }

        private static T CreatePiecesChain(IPieceColor pieceType)
        {
            var chain = new T();
            chain.PieceType = pieceType;
            return chain;
        }

        public abstract void PopulatePiecesChain(T chain, Queue<Vector2Int> coordsToCheck, IBoard board);
    }
}
