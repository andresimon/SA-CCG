﻿using UnityEngine;

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
            if (!isInit)
            {
                Settings.gameManager.SetState((!forceExit ? battlePhaseControl : null));
                Settings.gameManager.onPhaseCompleted.Raise();
                isInit = true;

                forceExit = !isBattleValid.IsValid();
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