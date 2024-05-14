using Bipolar.PuzzleBoard;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Bipolar.Match3
{
    public class MatchPredictor : MonoBehaviour
    {
        [SerializeField]
        private Matcher matcher;
        [SerializeField]
        private SceneBoard sceneBoard;

        //private readonly List<PiecesChain> chainsBuffer = new List<PiecesChain>();

        private readonly Dictionary<CoordsPair, List<PiecesChain>> possibleChainsObtainedBySwapping = new Dictionary<CoordsPair, List<PiecesChain>>();

        public void FindPossibleChains()
        {
            var boardData = sceneBoard.GetBoardState();

            bool isHexagonal = boardData.Layout == GridLayout.CellLayout.Hexagon;
            var directions = BoardHelper.GetDirections(isHexagonal);
            int directionsCount = directions.Count / 2;

            foreach (var chainsList in possibleChainsObtainedBySwapping.Values) 
                ListPool<PiecesChain>.Release(chainsList);
            
            possibleChainsObtainedBySwapping.Clear();

            foreach (var coord in sceneBoard.Board)
            {
                for (int dirIndex = 0; dirIndex < directionsCount; dirIndex++)
                {
                    Board.Copy(sceneBoard.Board, boardData);
                    var otherCoord = coord + BoardHelper.GetCorrectedDirection(coord, directions[dirIndex], isHexagonal);
                    if (boardData.ContainsCoord(otherCoord))
                        CheckIfSwappingPieceCreatesMatches(boardData, new CoordsPair(coord, otherCoord));
                }
            }
            if (possibleChainsObtainedBySwapping.Count == 0)
                Debug.LogError("No matches possible!");
        }

        private void CheckIfSwappingPieceCreatesMatches(IBoard board, CoordsPair swappedCoords)
        {
            board.SwapPieces(swappedCoords);

            var coordsQueue = new Queue<Vector2Int>();
            var chainsList = ListPool<PiecesChain>.Get();

            matcher.FindAndCreatePieceChains(board, chainsList, stackalloc Vector2Int[] {swappedCoords.firstCoord, swappedCoords.secondCoord});
            if (chainsList.Count > 0) 
            {
                //string message = $"If you swap {swappedCoords} you will get matches:";
                //foreach (var chain in chainsList)
                //    message += $"\n\t{chain}";
                //Debug.Log(message);
                possibleChainsObtainedBySwapping.Add(swappedCoords, chainsList);
            }
            else
            {
                ListPool<PiecesChain>.Release(chainsList);
            }
        }
    }
}
