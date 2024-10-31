using Bipolar.PuzzleBoard;
using System.Collections.Generic;
using UnityEngine;

namespace Bipolar.Match3
{
    public class PieceClearingChainProcessor : ChainsProcessor
    {
        [SerializeField]
        private SceneBoard sceneBoard;
        [SerializeField]
        private PiecesClearManager piecesClearManager;
        [SerializeField]
        private BoardController boardController;
        
        private readonly List<IPiece> clearedPieces = new List<IPiece>();

        protected virtual void Reset()
        {
            boardController = FindObjectOfType<BoardController>();
            piecesClearManager = FindObjectOfType<PiecesClearManager>();
            sceneBoard = FindObjectOfType<SceneBoard>();
        }

        public override void ProcessChains(IEnumerable<PiecesChain> chains)
        {
            clearedPieces.Clear();
            foreach (var chain in chains)
            {
                if (chain is TriosWithSquaresPiecesChain squaresChain && squaresChain.SquaresCount > 0)
                {
                    var squareBombPieceCoord = squaresChain.StartingCoord;
                }


                foreach (var coord in chain.PiecesCoords)
                {
                    if (sceneBoard.TryGetPiece(coord, out var piece))
                    {
                        if (piece is ICompoundPiece compoundPiece)
                        {
                            foreach (var property in compoundPiece.Properties)
                                ;// if (property is IApplicablePieceProperty applicableProperty)
                            ; // applicableProperty.Apply(coord, sceneBoard.Board)
                        }
                        if (piece == null)
                        {
                            Debug.LogError("Czemu tu jest null?");
                        }
                        piece.ClearPiece();
                        clearedPieces.Add(piece);
                    }
                }
            }

            var command = new ClearPiecesCommand(clearedPieces, piecesClearManager);
            boardController.RequestCommand(command);
        }
    }
}
