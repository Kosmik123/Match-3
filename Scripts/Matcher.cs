using Bipolar.PuzzleBoard;
using System.Collections.Generic;
using UnityEngine;

namespace Bipolar.Match3
{
    public class Matcher : MonoBehaviour
    {
        [SerializeField]
        private Board board;

        [SerializeField]
        private MatchingStrategy matchingStrategy;
        public MatchingStrategy MatchingStrategy
        {
            get => matchingStrategy;
            private set => matchingStrategy = value;
        }

        protected virtual void Reset()
        {
            board = FindObjectOfType<Board>();
        }

        public void SetMatchingStrategy<T>() where T : MatchingStrategy
        {
            matchingStrategy = ScriptableObject.CreateInstance<MatchingStrategy>();
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

    public class MatchPredictor : MonoBehaviour
    {
        [SerializeField]
        private Board board;
        [SerializeField]
        private MatchingStrategy matchingStrategy;

        public void FindPossibleChains()
        {
            var data = board.GetBoardState();

            bool isHexagonal = data.Layout == GridLayout.CellLayout.Hexagon;
            var directions = MatchingStrategy.GetLinesDirections(data.Layout);
            int directionsCount = directions.Count / 2; 
            
            foreach (var coord in board)
            {
                for (int dirIndex = 0; dirIndex < directionsCount; dirIndex++)
                {
                    var otherCoord = coord + BoardHelper.GetCorrectedDirection(coord, directions[dirIndex], isHexagonal);
                    CheckIfSwappingPieceCreatesMatches(coord, otherCoord, data);
                }
            }
        }

        public void CheckIfSwappingPieceCreatesMatches(Vector2Int pieceCoord1, Vector2Int pieceCoord2, BoardState boardData)
        {
            (boardData[pieceCoord1], boardData[pieceCoord2]) = (boardData[pieceCoord2], boardData[pieceCoord1]);
            (boardData[pieceCoord1], boardData[pieceCoord2]) = (boardData[pieceCoord2], boardData[pieceCoord1]);

            IPieceColor GetPieceTypeAtCoord(Vector2Int coord)
            {
                if (coord == pieceCoord1)
                    return boardData[pieceCoord2].Color;
                
                if (coord == pieceCoord2)
                    return boardData[pieceCoord1].Color;
                
                return boardData[coord].Color;
            }
        }
    }
}
