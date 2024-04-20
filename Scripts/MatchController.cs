using System.Collections.Generic;
using UnityEngine;

namespace Bipolar.Match3
{
    public class MatchController : MonoBehaviour
    {
        public event System.Action<PiecesChain> OnPiecesMatched;
        public event System.Action OnMatchingFailed;

        [SerializeField]
        private Matcher matcher;

        private readonly List<PiecesChain> chainList = new List<PiecesChain>();
        public IReadOnlyList<PiecesChain> Chains => chainList;

        protected virtual void Reset()
        {
            matcher = FindObjectOfType<Matcher>();
        }

        public bool FindMatches()
        {
            matcher.FindAndCreatePieceChains(chainList);
            foreach (var chain in chainList)
            {
                OnPiecesMatched?.Invoke(chain); 
            }
            if (chainList.Count <= 0)
                OnMatchingFailed?.Invoke();
            
#if UNITY_EDITOR
            var colorRandomizer = new System.Random(chainList.Count);
            foreach (var chain in chainList)
            {
                var color = Color.HSVToRGB((float)colorRandomizer.NextDouble(), 1, 1);
                color.a = 0.5f;
                chain.DrawDebug(matcher.BoardComponent, color, 2);
            }
#endif

            return chainList.Count > 0;
        }
    }
}
