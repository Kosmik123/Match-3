using Bipolar.PuzzleBoard;
using Bipolar.PuzzleBoard.General;
using Bipolar.PuzzleBoard.Rectangular;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bipolar.Match3
{
    [RequireComponent(typeof(GeneralBoard))]
    public class GeneralBoardMatcher : Matcher<IGeneralBoard>
    {
        private readonly Queue<Vector2Int> coordsToCheck = new Queue<Vector2Int>();

        public override void FindAndCreatePieceChains()
        {
            var board = Board;
            pieceChains.Clear();
            foreach (var coord in board.Coords)
            {
                if (pieceChains.FirstOrDefault(chain => chain.Contains(coord)) != null)
                    continue;

                CreatePiecesChain(coord, coordsToCheck);
            }
        }
    }
}
