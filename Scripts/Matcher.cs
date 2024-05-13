using Bipolar.PuzzleBoard;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bipolar.Match3
{
    public interface IMatcher
    {
        void FindAndCreatePieceChains(IList<PiecesChain> pieceChainsBuffer, System.ReadOnlySpan<Vector2Int> startingCoords);
        void SetMatchingStrategy<T>() where T : MatchingStrategy;
    }

    public class Matcher : MonoBehaviour, IMatcher
    {
        [SerializeField]
        private SceneBoard sceneBoard;
        public SceneBoard SceneBoard => sceneBoard;

        [SerializeField]
        private MatchingStrategy matchingStrategy;
        protected IMatchingStrategy MatchingStrategy
        {
            get => matchingStrategy;
            set => matchingStrategy = (MatchingStrategy)value;
        }

        protected virtual void Reset()
        {
            sceneBoard = FindObjectOfType<SceneBoard>();
        }

        public void SetMatchingStrategy<T>() where T : MatchingStrategy
        {
            matchingStrategy = ScriptableObject.CreateInstance<T>();
        }

        private readonly Queue<Vector2Int> coordsToCheck = new Queue<Vector2Int>();

        public void FindAndCreatePieceChains(IList<PiecesChain> pieceChainsBuffer, System.ReadOnlySpan<Vector2Int> startingCoords)
        {
            pieceChainsBuffer.Clear();
            foreach (var coord in startingCoords)
                TryAddChainWithCoord(pieceChainsBuffer, coord, coordsToCheck);
            AddChainsFromBoard(pieceChainsBuffer);
        }

        private void AddChainsFromBoard(IList<PiecesChain> pieceChainsBuffer)
        {
            foreach (var coord in sceneBoard.Board)
                TryAddChainWithCoord(pieceChainsBuffer, coord, coordsToCheck);
        }

        private bool TryAddChainWithCoord(IList<PiecesChain> pieceChainsBuffer, Vector2Int coord, Queue<Vector2Int> coordsQueue = null)
        {
            if (pieceChainsBuffer.FirstOrDefault(chain => chain.Contains(coord)) != null)
                return false;

            if (TryCreatePiecesChain(coord, out var chain, coordsQueue) == false)
                return false;

            pieceChainsBuffer.Add(chain);
            return true;
        }

        private bool TryCreatePiecesChain(Vector2Int startingCoord, out PiecesChain resultChain, Queue<Vector2Int> coordsToCheck = null)
        {
            coordsToCheck ??= new Queue<Vector2Int>();
            coordsToCheck.Clear();
            resultChain = MatchingStrategy.GetPiecesChain(startingCoord, sceneBoard.Board, coordsToCheck);
            return resultChain.IsMatchFound;
        }
    }
}
