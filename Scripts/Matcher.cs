using Bipolar.PuzzleBoard;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bipolar.Match3
{
    public class Matcher : MonoBehaviour
    {
        [SerializeField]
        private BoardComponent boardComponent;
        public BoardComponent BoardComponent => boardComponent;

        [SerializeField]
        private MatchingStrategy matchingStrategy;
        public IMatchingStrategy MatchingStrategy
        {
            get => matchingStrategy;
            private set => matchingStrategy = (MatchingStrategy)value;
        }

        protected virtual void Reset()
        {
            boardComponent = FindObjectOfType<BoardComponent>();
        }

        public void SetMatchingStrategy<T>() where T : MatchingStrategy
        {
            matchingStrategy = ScriptableObject.CreateInstance<T>();
        }

        public bool TryAddChainWithCoord(IList<PiecesChain> pieceChainsBuffer, Vector2Int coord, Queue<Vector2Int> coordsQueue = null)
        {
            if (pieceChainsBuffer.FirstOrDefault(chain => chain.Contains(coord)) != null)
                return false;

            if (TryCreatePiecesChain(coord, out var chain, coordsQueue) == false)
                return false;

            pieceChainsBuffer.Add(chain);
            return true;
        }

        protected bool TryCreatePiecesChain(Vector2Int startingCoord, out PiecesChain resultChain, Queue<Vector2Int> coordsToCheck = null)
        {
            coordsToCheck ??= new Queue<Vector2Int>();
            coordsToCheck.Clear();
            coordsToCheck.Enqueue(startingCoord);

            resultChain = MatchingStrategy.GetPiecesChain(coordsToCheck, boardComponent.Board);
            return resultChain.IsMatchFound;
        }

        private readonly Queue<Vector2Int> coordsToCheck = new Queue<Vector2Int>();

        public void FindAndCreatePieceChains(IList<PiecesChain> pieceChainsBuffer, params Vector2Int[] startingCoords)
        {
            pieceChainsBuffer.Clear();
            foreach (var coord in startingCoords)
                TryAddChainWithCoord(pieceChainsBuffer, coord, coordsToCheck);

            AddChainsFromBoard(pieceChainsBuffer);
        }

        public void FindAndCreatePieceChains(IList<PiecesChain> pieceChainsBuffer, Vector2Int startingCoord)
        {
            pieceChainsBuffer.Clear();
            TryAddChainWithCoord(pieceChainsBuffer, startingCoord, coordsToCheck);
            AddChainsFromBoard(pieceChainsBuffer);
        }

        private void AddChainsFromBoard(IList<PiecesChain> pieceChainsBuffer)
        {
            foreach (var coord in boardComponent.Board)
                TryAddChainWithCoord(pieceChainsBuffer, coord, coordsToCheck);
        }
    }
}
