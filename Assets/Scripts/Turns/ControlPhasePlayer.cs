using UnityEngine;

namespace Legendary
{

    [CreateAssetMenu(menuName = "Turns/Control Phase Player")]
    public class ControlPhasePlayer : Phase
    {
        public GameStates.GameState playerControlState;

        public override bool IsComplete()
        {
            if (forceExit)
            {
                forceExit = false;
                return true;
            }

            return false;
        }
       
        public override void OnStartPhase()
        {
            if (!isInit)
            {
                Settings.gameManager.SetState(playerControlState);
                Settings.gameManager.onPhaseCompleted.Raise();
                isInit = true;
            }
        }

        public override void OnEndPhase()
        {
            if (isInit)
            {
                Settings.gameManager.SetState(null);
                isInit = false;
            }
        }

    }

}