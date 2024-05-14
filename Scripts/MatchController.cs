using Bipolar.PuzzleBoard;
using System.Collections.Generic;
using UnityEngine;

namespace Bipolar.Match3
{
    public class MatchController : MonoBehaviour
    {
        public event System.Action<PiecesChain> OnPiecesMatched;
        public event System.Action OnMatchingFailed;

        [SerializeField]
        private Matcher _matcher;
        protected IMatcher Matcher => _matcher;

        [SerializeField]
        private SceneBoard sceneBoard;

        private readonly List<PiecesChain> chainList = new List<PiecesChain>();
        public IReadOnlyList<PiecesChain> Chains => chainList;

        protected virtual void Reset()
        {
            _matcher = FindObjectOfType<Matcher>();
        }

        public bool FindMatches(System.ReadOnlySpan<Vector2Int> startingCoords)
        {
            Matcher.FindAndCreatePieceChains(sceneBoard.Board, chainList, startingCoords);
            foreach (var chain in chainList)
            {
                OnPiecesMatched?.Invoke(chain); 
            }

            if (chainList.Count <= 0)
                OnMatchingFailed?.Invoke();

#if UNITY_EDITOR
            var previousRandomState = Random.state;
            Random.InitState(chainList.Count);
            foreach (var chain in chainList)
            {
                var color = Color.HSVToRGB(Random.value, 1, 1);
                color.a = 0.5f;
                chain.DrawDebug(sceneBoard, color, 2);
            }
            Random.state = previousRandomState;
#endif
            return chainList.Count > 0;
        }
    }
}
