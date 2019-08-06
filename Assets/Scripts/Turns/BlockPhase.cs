﻿using UnityEngine;

namespace Legendary
{

    [CreateAssetMenu(menuName = "Turns/BlockPhase")]
    public class BlockPhase : Phase
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
            forceExit = true;
            return;

            GameManager gm = Settings.gameManager;

            gm.SetState(playerControlState);
            gm.onPhaseCompleted.Raise();

            PlayerHolder e = gm.GetEnemyOf(gm.currentPlayer);
            if (e.attackingCards.Count == 0)
            {
                forceExit = true;
                return;
            }
        }

    }

}