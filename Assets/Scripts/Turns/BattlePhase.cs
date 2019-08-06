using UnityEngine;

namespace Legendary
{

    [CreateAssetMenu(menuName = "Turns/Battle Phase Player")]
    public class BattlePhase : Phase
    {
        public GameStates.GameState battlePhaseControl;
        public Condition isBattleValid;

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
            forceExit = true;
            return;

            forceExit = !isBattleValid.IsValid();
            Settings.gameManager.SetState((!forceExit ? battlePhaseControl : null));
            Settings.gameManager.onPhaseCompleted.Raise();
        }

    }

}