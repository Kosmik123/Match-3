using Bipolar.PuzzleBoard.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bipolar.Match3
{
    public class MatchController : MonoBehaviour
    {
        public event System.Action<PiecesChain> OnPiecesMatched;
        public event System.Action OnMatchingFailed;

        public event PiecesSwapEventHandler OnPiecesSwapped;

        [SerializeField]
        private BoardComponent boardComponent;

        [SerializeField]
        private Matcher matcher;

        private System.Action swapEndedCallback;
        private readonly List<PiecesChain> chainList = new List<PiecesChain>();

        protected virtual void Reset()
        {
            boardComponent = FindObjectOfType<BoardComponent>();
            matcher = FindObjectOfType<Matcher>();
        }

        public bool TrySwapPieces(Vector2Int pieceCoord1, Vector2Int pieceCoord2)
        {
            SwapPieces(pieceCoord1, pieceCoord2);
            FindMatches();
            bool wasCorrectMove = chainList.Count > 0;
            if (wasCorrectMove == false)
            {
                SwapPieces(pieceCoord2, pieceCoord1);
                OnMatchingFailed?.Invoke();
            }

            return wasCorrectMove;


            //boardController.PiecesMovementManager.OnAllPiecesMovementStopped += PiecesMovementManager_OnAllPiecesMovementStopped;
            swapEndedCallback = () =>
            {
                swapEndedCallback = null;
                FindMatches();
                bool wasCorrectMove = chainList.Count > 0;
                if (wasCorrectMove == false)
                {
                    SwapPieces(pieceCoord2, pieceCoord1);
                    OnMatchingFailed?.Invoke();
                }
            };


            StartCoroutine(TEST_CheckMatchesAfterSwapping(pieceCoord1, pieceCoord2));
        }

        private IEnumerator TEST_CheckMatchesAfterSwapping(Vector2Int pieceCoord1, Vector2Int pieceCoord2)
        {
            yield return new WaitForSeconds(0.1f);
            FindMatches();
            bool wasCorrectMove = chainList.Count > 0;
            if (wasCorrectMove == false)
                SwapPieces(pieceCoord1, pieceCoord2);
        }

        private void SwapPieces(Vector2Int pieceCoord1, Vector2Int pieceCoord2)
        {
            Debug.Log($"Pieces at {pieceCoord1} and {pieceCoord2} swapped");
            boardComponent.SwapPieces(pieceCoord1, pieceCoord2);
            OnPiecesSwapped?.Invoke(pieceCoord1, pieceCoord2);
        }

        private void PiecesMovementManager_OnAllPiecesMovementStopped()
        {
            // boardController.PiecesMovementManager.OnAllPiecesMovementStopped -= PiecesMovementManager_OnAllPiecesMovementStopped;
            swapEndedCallback?.Invoke();
        }

        public void FindMatches()
        {
            matcher.FindAndCreatePieceChains(chainList);
            foreach (var chain in chainList)
            {
                OnPiecesMatched?.Invoke(chain); 
            }

#if UNITY_EDITOR
            var colorRandomizer = new System.Random(chainList.Count);
            foreach (var chain in chainList)
            {
                var color = Color.HSVToRGB((float)colorRandomizer.NextDouble(), 1, 1);
                color.a = 0.5f;
                chain.DrawDebug(boardComponent, color, 2);
            }
#endif
        }
    }
}
