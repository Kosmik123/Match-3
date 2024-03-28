using Bipolar.PuzzleBoard.Rectangular;
using System.Collections.Generic;
using UnityEngine;

namespace Bipolar.Match3
{
    [RequireComponent(typeof(RectangularBoard))]
    public class RectangularBoardMatcher : Matcher<IRectangularBoard>
    {
        private readonly Queue<Vector2Int> coordsToCheck = new Queue<Vector2Int>();

        public override void FindAndCreatePieceChains(List<PiecesChain> pieceChains)
        {
            var board = TypedBoard;
            pieceChains.Clear();
            for (int j = 0; j < board.Dimensions.y; j++)
            {
                for (int i = 0; i < board.Dimensions.x; i++)
                {
                    var coord = new Vector2Int(i, j);
                    TryAddChainWithCoord(pieceChains, coord, coordsToCheck);
                }
            }
        }
    }
}
