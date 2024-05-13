using Bipolar.PuzzleBoard;
using System.Collections.Generic;
using UnityEngine;

namespace Bipolar.Match3
{
    public class MatchPredictor : MonoBehaviour
    {
        [SerializeField]
        private Matcher matcher;

        private readonly List<PiecesChain> chainsBuffer = new List<PiecesChain>();

        public void FindPossibleChains()
        {
            var boardData = matcher.SceneBoard.GetBoardState();

            bool isHexagonal = boardData.Layout == GridLayout.CellLayout.Hexagon;
            var directions = BoardHelper.GetDirections(isHexagonal);
            int directionsCount = directions.Count / 2;

            foreach (var coord in matcher.SceneBoard.Board)
            {
                for (int dirIndex = 0; dirIndex < directionsCount; dirIndex++)
                {
                    var otherCoord = coord + BoardHelper.GetCorrectedDirection(coord, directions[dirIndex], isHexagonal);
                    if (boardData.ContainsCoord(otherCoord))
                        CheckIfSwappingPieceCreatesMatches(new CoordsPair(coord, otherCoord), boardData);
                }
            }
        }

        public void CheckIfSwappingPieceCreatesMatches(CoordsPair coordsPair, IBoard board)
        {
            board.SwapPieces(coordsPair);

            var coordsQueue = new Queue<Vector2Int>();
            foreach (var coord in board)
            {
                coordsQueue.Clear();
                //matcher.FindAndCreatePieceChains(chainsBuffer);
                // matcher.MatchingStrategy.GetPiecesChain(coord, board, coordsQueue);
            }

            board.SwapPieces(coordsPair);
        }
    }
}
