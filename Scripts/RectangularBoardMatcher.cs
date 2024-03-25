using Bipolar.PuzzleBoard;
using Bipolar.PuzzleBoard.Rectangular;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bipolar.Match3
{
    [RequireComponent(typeof(RectangularBoard))]
    public class RectangularBoardMatcher : Matcher<IRectangularBoard>
    {
        private readonly Queue<Vector2Int> coordsToCheck = new Queue<Vector2Int>();

        public override void FindAndCreatePieceChains(IRectangularBoard board)
        {
            pieceChains.Clear();
            for (int j = 0; j < board.Dimensions.y; j++)
            {
                for (int i = 0; i < board.Dimensions.x; i++)
                {
                    Vector2Int coord = new Vector2Int(i, j);
                    if (pieceChains.FirstOrDefault(chain => chain.Contains(coord)) != null)
                        continue;

                    CreatePiecesChain(board, coord, coordsToCheck);
                }
            }
        }
    }
}
