using Bipolar.PuzzleBoard;
using Bipolar.PuzzleBoard.Rectangular;
using System.Collections.Generic;
using UnityEngine;

namespace Bipolar.Match3
{
    [RequireComponent(typeof(Board))]
    public abstract class Matcher : MonoBehaviour
    {
        [SerializeField]
        private MatchingStrategy matchingStrategy;
        public MatchingStrategy MatchingStrategy
        {
            get => matchingStrategy;
            set => matchingStrategy = value;
        }

        public void SetMatchingStrategy<T>() where T : MatchingStrategy
        {
            matchingStrategy = ScriptableObject.CreateInstance<MatchingStrategy>();
        }

        public abstract IBoard Board { get; }
        public abstract void FindAndCreatePieceChains(List<PiecesChain> chainList);
    }

    [RequireComponent(typeof(Board))]
    public abstract class Matcher<TBoard> : Matcher
        where TBoard : class, IBoard
    {
        private TBoard _board;
        public TBoard TypedBoard
        {
            get
            {
                if (_board == null) 
                    _board = GetComponent<TBoard>(); 
                return _board;
            }
        }
        public override IBoard Board => TypedBoard;

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

            resultChain = MatchingStrategy.GetPiecesChain(coordsToCheck, Board);
            return resultChain.IsMatchFound;
        }

        private void OnDrawGizmos()
        {
            //if (Board != null)
            //{
            //    var random = new System.Random(PieceChains.Count);
            //    foreach (var chain in PieceChains)
            //    {
            //        var color = Color.HSVToRGB((float)random.NextDouble(), 1, 1);
            //        color.a = 0.5f;
            //        Gizmos.color = color;
            //        chain.DrawGizmo(Board);
            //    }
            //}
        }
    }

    public class MatchPredictor : MonoBehaviour
    {
        [SerializeField]
        private RectangularBoard board;

        public void FindPossibleChains()
        {
            bool isHexagonal = board.Grid.cellLayout == GridLayout.CellLayout.Hexagon;
            var directions = MatchingStrategy.GetLinesDirections(board.Grid.cellLayout);
            int directionsCount = directions.Count / 2; 
            
            for (int y = 0; y < board.Dimensions.y - 1; y++)
            {
                for (int x = 0; x < board.Dimensions.x - 1; x++)
                {
                    var coord = new Vector2Int(x, y);
                    for (int dirIndex = 0; dirIndex < directionsCount; dirIndex++)
                    {
                        var otherCoord = coord + BoardHelper.GetCorrectedDirection(coord, directions[dirIndex], isHexagonal);
                        CheckIfSwappingPieceCreatesMatches(coord, otherCoord, board.Data);
                    }
                }
            }
        }

        public void CheckIfSwappingPieceCreatesMatches(Vector2Int pieceCoord1, Vector2Int pieceCoord2, BoardData boardData)
        { 

















            IPieceType GetPieceTypeAtCoord(Vector2Int coord)
            {
                if (coord == pieceCoord1)
                    return boardData[pieceCoord2].Type;
                
                if (coord == pieceCoord2)
                    return boardData[pieceCoord1].Type;
                
                return boardData[coord].Type;
            }
        }
    }
}
