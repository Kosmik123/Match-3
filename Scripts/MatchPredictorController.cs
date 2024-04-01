using UnityEngine;

namespace Bipolar.Match3
{
    public class MatchPredictorController : MonoBehaviour
    {
        [SerializeField]
        private MatchPredictor matchPredictor;
        [SerializeField]
        private MatchController matchController;

        protected virtual void Reset()
        {
            matchPredictor = FindObjectOfType<MatchPredictor>();
        }

        private void OnEnable()
        {
            matchController.OnPiecesMatched += MatchController_OnPiecesMatched;    
        }

        private void MatchController_OnPiecesMatched(PiecesChain piecesChain)
        {

        }

        private void OnDisable()
        {
            matchController.OnPiecesMatched += MatchController_OnPiecesMatched;    
            
        }
    }
}
