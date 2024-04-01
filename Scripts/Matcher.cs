using Bipolar.PuzzleBoard;
using System.Collections.Generic;
using UnityEngine;

namespace Bipolar.Match3
{
    public class Matcher : MonoBehaviour
    {
        [SerializeField]
        private Board board;
        public Board Board => board;

        [SerializeField]
        private MatchingStrategy matchingStrategy;
        public IMatchingStrategy MatchingStrategy
        {
            get => matchingStrategy;
            private set => matchingStrategy = (MatchingStrategy)value;
        }

        protected virtual void Reset()
        {
            board = FindObjectOfType<Board>();
        }

        public void SetMatchingStrategy<T>() where T : MatchingStrategy
        {
            matchingStrategy = ScriptableObject.CreateInstance<T>();
        }

        public bool TryAddChainWithCoord(List<PiecesChain> piecesChains, Vector2Int coord, Queue<Vector2Int> coordsQueue = null)
        {
            if (piecesChains.Find(chain => chain.Contains(coord)) != null)
                return false;

            if (TryCreatePiecesChain(coord, out var chain, coordsQueue) == false)
                return false;

            piecesChains.Add(chain);
            return true;
        }

        protected bool TryCreatePiecesChain(Vector2Int startingCoord, out PiecesChain resultChain, Queue<Vector2Int> coordsToCheck = null)
        {
            coordsToCheck ??= new Queue<Vector2Int>();
            coordsToCheck.Clear();
            coordsToCheck.Enqueue(startingCoord);

            resultChain = MatchingStrategy.GetPiecesChain(coordsToCheck, board);
            return resultChain.IsMatchFound;
        }

        private readonly Queue<Vector2Int> coordsToCheck = new Queue<Vector2Int>();

        public void FindAndCreatePieceChains(List<PiecesChain> pieceChains)
        {
            pieceChains.Clear();
            foreach (var coord in board)
            {
                TryAddChainWithCoord(pieceChains, coord, coordsToCheck);
            }
        }
    }
}
