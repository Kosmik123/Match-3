using Bipolar.PuzzleBoard;
using System.Collections.Generic;
using UnityEngine;

namespace Bipolar.Match3
{
    public interface IMatcher
    {
        void FindAndCreatePieceChains(IReadOnlyBoard board, IList<PiecesChain> pieceChainsBuffer, System.ReadOnlySpan<Vector2Int> startingCoords);
        void SetMatchingStrategy<T>() where T : MatchingStrategy;
    }

    public class Matcher : MonoBehaviour, IMatcher
    {
        [SerializeField]
        private MatchingStrategy matchingStrategy;
        protected IMatchingStrategy MatchingStrategy
        {
            get => matchingStrategy;
            set => matchingStrategy = (MatchingStrategy)value;
        }

        public void SetMatchingStrategy<T>() where T : MatchingStrategy
        {
            matchingStrategy = ScriptableObject.CreateInstance<T>();
        }

        private readonly Queue<Vector2Int> coordsToCheck = new Queue<Vector2Int>();

        public void FindAndCreatePieceChains(IReadOnlyBoard board, IList<PiecesChain> pieceChainsBuffer, System.ReadOnlySpan<Vector2Int> startingCoords)
        {
            pieceChainsBuffer.Clear();
            foreach (var coord in startingCoords)
                TryAddChainWithCoord(board, pieceChainsBuffer, coord, coordsToCheck);
            AddChainsFromBoard(board, pieceChainsBuffer);
        }

        private void AddChainsFromBoard(IReadOnlyBoard board, IList<PiecesChain> pieceChainsBuffer)
        {
            foreach (var coord in board)
                TryAddChainWithCoord(board, pieceChainsBuffer, coord, coordsToCheck);
        }

        private bool TryAddChainWithCoord(IReadOnlyBoard board, IList<PiecesChain> pieceChainsBuffer, Vector2Int coord, Queue<Vector2Int> coordsQueue = null)
        {
            if (HasChainForCoordBeenFound(pieceChainsBuffer, coord))
                return false;

            if (TryCreatePiecesChain(board, coord, out var chain, coordsQueue) == false)
                return false;

            pieceChainsBuffer.Add(chain);
            return true;
        }

        private bool TryCreatePiecesChain(IReadOnlyBoard board, Vector2Int startingCoord, out PiecesChain resultChain, Queue<Vector2Int> coordsToCheck = null)
        {
            coordsToCheck ??= new Queue<Vector2Int>();
            coordsToCheck.Clear();
            resultChain = MatchingStrategy.GetPiecesChain(startingCoord, board, coordsToCheck);
            return resultChain.IsMatchFound;
        }

        public static bool HasChainForCoordBeenFound(IList<PiecesChain> pieceChainsBuffer, Vector2Int coord)
        {
            foreach (var chain in pieceChainsBuffer)
                if (chain.Contains(coord)) 
                    return true;

            return false;
        }
    }
}
