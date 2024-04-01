using Bipolar.PuzzleBoard;
using System.Collections.Generic;
using UnityEngine;

namespace Bipolar.Match3
{
    public class MatchPredictor : MonoBehaviour
    {
        [SerializeField]
        private Matcher matcher;

        public void FindPossibleChains()
        {
            var boardData = matcher.Board.GetBoardState();

            bool isHexagonal = boardData.Layout == GridLayout.CellLayout.Hexagon;
            var directions = BoardHelper.GetDirections(boardData.Layout);
            int directionsCount = directions.Count / 2; 

            foreach (var coord in matcher.Board)
            {
                for (int dirIndex = 0; dirIndex < directionsCount; dirIndex++)
                {
                    var otherCoord = coord + BoardHelper.GetCorrectedDirection(coord, directions[dirIndex], isHexagonal);
                    if (boardData.ContainsCoord(otherCoord))
                        CheckIfSwappingPieceCreatesMatches(coord, otherCoord, boardData);
                }
            }
        }

        public void CheckIfSwappingPieceCreatesMatches(Vector2Int pieceCoord1, Vector2Int pieceCoord2, BoardState boardData)
        {
            (boardData[pieceCoord1], boardData[pieceCoord2]) = (boardData[pieceCoord2], boardData[pieceCoord1]);

            var coordsQueue = new Queue<Vector2Int>();
            foreach (var coord in boardData)
            {
                coordsQueue.Clear();
                coordsQueue.Enqueue(coord);
                matcher.MatchingStrategy.GetPiecesChain(coordsQueue, boardData);
            }
            
            (boardData[pieceCoord1], boardData[pieceCoord2]) = (boardData[pieceCoord2], boardData[pieceCoord1]);
        }
    }
}
