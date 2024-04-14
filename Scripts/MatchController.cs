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
        private BoardController boardController;

        [SerializeField]
        private Matcher matcher;

        private System.Action swapEndedCallback;
        private readonly List<PiecesChain> chainList = new List<PiecesChain>();

        protected virtual void Reset()
        {
            boardController = FindObjectOfType<BoardController>();
            matcher = FindObjectOfType<Matcher>();
        }

        public void StartSwappingTokens(Vector2Int pieceCoord1, Vector2Int pieceCoord2)
        {
            boardController.PiecesMovementManager.OnAllPiecesMovementStopped += PiecesMovementManager_OnAllPiecesMovementStopped;
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

            SwapPieces(pieceCoord1, pieceCoord2);
        }

        private void SwapPieces(Vector2Int pieceCoord1, Vector2Int pieceCoord2)
        {
            (boardController.BoardComponent.Board[pieceCoord2], boardController.BoardComponent.Board[pieceCoord1]) = (boardController.BoardComponent.Board[pieceCoord1], boardController.BoardComponent.Board[pieceCoord2]);
        }

        private void PiecesMovementManager_OnAllPiecesMovementStopped()
        {
            boardController.PiecesMovementManager.OnAllPiecesMovementStopped -= PiecesMovementManager_OnAllPiecesMovementStopped;
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
                chain.DrawDebug(boardController.BoardComponent, color, 2);
            }
#endif
        }
    }
}
