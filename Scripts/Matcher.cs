using Bipolar.PuzzleBoard;
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

        protected bool TryCreatePiecesChain(Vector2Int startingCoord, out PiecesChain resultChain, Queue<Vector2Int> coordsToCheck = null)
        {
            coordsToCheck ??= new Queue<Vector2Int>();
            coordsToCheck.Clear();
            coordsToCheck.Enqueue(startingCoord);

            resultChain = MatchingStrategy.GetPiecesChain(coordsToCheck, Board);

            //// matching strategy begin
            //var chain = new TriosPiecesChain();
            //chain.PieceType = Board.GetPiece(startingCoord).Type;
            //DefaultMatchingStrategy.FindMatches(Board, chain, coordsToCheck);
            //// matching strategy end

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
}
