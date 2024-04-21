﻿using Bipolar.PuzzleBoard;
using Bipolar.PuzzleBoard.Components;
using System.Collections.Generic;
using UnityEngine;

namespace Bipolar.Match3
{
    public class MatchingManager : MonoBehaviour
    {
        public event PiecesSwapEventHandler OnPiecesSwapped;

        [SerializeField]
        private BoardController boardController;
        [SerializeField]
        private SwapRequester swapRequester;
        [SerializeField]
        private MatchController matchController;
        [SerializeField]
        private PiecesClearManager piecesClearManager;

        [SerializeField]
        private int combo;
        public int Combo => combo;

        [SerializeField]
        private MatchPredictor testMatchPredictor;

        protected virtual void Reset()
        {
            boardController = FindObjectOfType<BoardController>();
            swapRequester = FindObjectOfType<SwapRequester>();
            matchController = FindObjectOfType<MatchController>();
            piecesClearManager = FindObjectOfType<PiecesClearManager>();
        }

        private void OnEnable()
        {
            // boardController.OnPiecesColapsed += BoardController_OnPiecesColapsed;
            swapRequester.OnSwapRequested += SwapManager_OnSwapRequested;
            //piecesClearManager.OnAllPiecesCleared += PiecesClearManager_OnAllPiecesCleared;
            //matchController.OnPiecesMatched += MatchController_OnPiecesMatched;
        }

        private void Start()
        {
            do
            {
                boardController.Collapse();
                bool matchesAtStart = matchController.FindMatches();
                if (matchesAtStart == false)
                    return;

                ClearChains(matchController.Chains);
            }
            while (true);
        }

        private void SwapManager_OnSwapRequested(Vector2Int pieceCoord1, Vector2Int pieceCoord2)
        {
            Debug.Log($"Swap of {pieceCoord1} and {pieceCoord2} requested");
            if (boardController.IsBusy)
                return;

            //if (piecesClearManager.CurrentlyClearedPiecesCount > 0)
            //    return;

            if (TrySwapAndMatch(pieceCoord1, pieceCoord2) == false)
            {
                SwapPieces(pieceCoord2, pieceCoord1);
            }
        }

        private bool TrySwapAndMatch(Vector2Int pieceCoord1, Vector2Int pieceCoord2)
        {
            combo = 0;
            SwapPieces(pieceCoord1, pieceCoord2);
            bool success = false;
            do 
            {
                combo++;
                bool matched = matchController.FindMatches();
                if (matched == false)
                    break;

                success = true;
                ClearChains(matchController.Chains);

                boardController.Collapse();
            }
            while (true);
            return success;
        }

        private void ClearChains(IReadOnlyList<PiecesChain> chains)
        {
            var piecesToClear = new List<Piece>(); 
            foreach (var chain in chains)
            {
                foreach (var coord in chain.PiecesCoords)
                {
                    piecesToClear.Add(boardController.BoardComponent.GetPiece(coord));
                }
            }

            piecesClearManager.ClearPieces(piecesToClear);
        }

        private void PiecesClearManager_OnAllPiecesCleared()
        {
            boardController.Collapse();
            testMatchPredictor?.FindPossibleChains();
        }

        private void SwapPieces(Vector2Int pieceCoord1, Vector2Int pieceCoord2)
        {
            Debug.Log($"Pieces at {pieceCoord1} and {pieceCoord2} swapped");
            boardController.BoardComponent.SwapPieces(pieceCoord1, pieceCoord2);
            OnPiecesSwapped?.Invoke(pieceCoord1, pieceCoord2);
        }
        
        private void OnDisable()
        {
            // boardController.OnPiecesColapsed -= BoardController_OnPiecesColapsed;
            swapRequester.OnSwapRequested -= SwapManager_OnSwapRequested;
            //piecesClearManager.OnAllPiecesCleared -= PiecesClearManager_OnAllPiecesCleared;
        }
    }
}
