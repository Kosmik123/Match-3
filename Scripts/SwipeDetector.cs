﻿using Bipolar.PuzzleBoard;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Bipolar.Match3
{
    public class SwipeDetector : MonoBehaviour, IDragHandler, IEndDragHandler
    {
        public delegate void TokenSwipeEventHandler(Vector2Int pieceCoord, Vector2Int direction);
        public event TokenSwipeEventHandler OnPieceSwiped;

        [SerializeField]
        private Board board;

        [SerializeField]
        private float releaseDetectionDistance = 0.5f;
        public float ReleaseDetectionDistance
        {
            get => releaseDetectionDistance;
            set => releaseDetectionDistance = value;
        }

        [SerializeField]
        private float dragDetectionDistance = 1; 
        public float DragDetectionDistance
        {
            get => dragDetectionDistance;
            set
            {
                dragDetectionDistance = value;
                sqrDragDetectionDistance = dragDetectionDistance * dragDetectionDistance;
            }
        }
        private float sqrDragDetectionDistance;

        [SerializeField]
        private bool disallowDiagonalSwipes = true;

        private bool hasDragged = false;

        protected virtual void Reset()
        {
            board = FindObjectOfType<Board>();   
        }

        private void Awake()
        {
            hasDragged = false;
            DragDetectionDistance = DragDetectionDistance;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (hasDragged)
                return;

            var pressWorldPosition = eventData.pointerPressRaycast.worldPosition;
            var pieceCoord = board.WorldToCoord(pressWorldPosition);
            if (board.ContainsCoord(pieceCoord) == false)
                return;

            var pointerCurrentRaycast = eventData.pointerCurrentRaycast;
            if (pointerCurrentRaycast.isValid == false)
                return;

            var currentWorldPosition = pointerCurrentRaycast.worldPosition;
            var delta = currentWorldPosition - pressWorldPosition;
            if (delta.sqrMagnitude > sqrDragDetectionDistance)
            {
                hasDragged = true;
                var direction = GetDirectionFromMove(pieceCoord, delta);
                OnPieceSwiped?.Invoke(pieceCoord, direction);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (hasDragged)
            {
                hasDragged = false;
                return;
            }
            
            var pressWorldPosition = eventData.pointerPressRaycast.worldPosition;
            var pieceCoord = board.WorldToCoord(pressWorldPosition);
            if (board.ContainsCoord(pieceCoord) == false)
                return;

            var pointerCurrentRaycast = eventData.pointerCurrentRaycast;
            if (pointerCurrentRaycast.isValid == false)
                return;

            var releaseWorldPosition = pointerCurrentRaycast.worldPosition;
            var delta = releaseWorldPosition - pressWorldPosition;
            if (delta.sqrMagnitude > releaseDetectionDistance * releaseDetectionDistance)
            {
                var direction = GetDirectionFromMove(pieceCoord, delta);
                OnPieceSwiped?.Invoke(pieceCoord, direction);
            }
        }

        private Vector2Int GetDirectionFromMove(Vector2Int startCoord, Vector2 moveDelta)
        {
            if (disallowDiagonalSwipes)
            {
                if (Mathf.Abs(moveDelta.y) < Mathf.Abs(moveDelta.x))
                    moveDelta.y = 0;
                else
                    moveDelta.x = 0;
            }

            moveDelta.Normalize();
            moveDelta.Scale(board.Grid.cellSize + board.Grid.cellGap);

            var startPosition = board.CoordToWorld(startCoord);
            var endPosition = startPosition + (Vector3)moveDelta;
            var endCoord = board.WorldToCoord(endPosition);
           
            var direction = endCoord - startCoord;
            return direction;
        }

        private void OnValidate()
        {
            DragDetectionDistance = DragDetectionDistance;
        }
    }
}
