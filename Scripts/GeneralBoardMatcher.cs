using Bipolar.PuzzleBoard.General;
using System.Collections.Generic;
using UnityEngine;

namespace Bipolar.Match3
{
    [RequireComponent(typeof(GeneralBoard))]
    public class GeneralBoardMatcher : Matcher<IGeneralBoard>
    {
        private readonly Queue<Vector2Int> coordsToCheck = new Queue<Vector2Int>();

        public override void FindAndCreatePieceChains()
        {
            var board = TypedBoard;
            pieceChains.Clear();
            foreach (var coord in board.Coords)
            {
                if (pieceChains.Find(chain => chain.Contains(coord)) != null)
                    continue;

                if (TryCreatePiecesChain(coord, out var chain, coordsToCheck))
                    pieceChains.Add(chain);
            }
        }
    }
}
