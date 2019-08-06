using UnityEngine;

namespace Legendary
{

    [CreateAssetMenu(menuName = "Turns/Control Phase Player")]
    public class ControlPhasePlayer : Phase
    {
        public GameStates.GameState playerControlState;

        public PlayerAction[] turnStartActions;

       // public PlayerAction OnStartAction;

        public override bool IsComplete()
        {
            if (forceExit)
            {
                forceExit = false;
                return true;
            }

            playerControlState.Tick(Time.deltaTime);

            return false;
        }
       
        public override void OnStartPhase()
        {
            Settings.gameManager.SetState(playerControlState);
            Settings.gameManager.onPhaseCompleted.Raise();

            foreach (PlayerAction action in turnStartActions)
            {
                action.Execute(Settings.gameManager.localPlayer);
            }
        }

    }

}