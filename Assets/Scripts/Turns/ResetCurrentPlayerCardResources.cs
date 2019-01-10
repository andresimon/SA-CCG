using UnityEngine;
using System.Collections;

namespace Legendary
{
    [CreateAssetMenu(menuName = "Turns/ResetCurrentPlayerCardResources")]
    public class ResetCurrentPlayerCardResources : Phase
    {
        public override bool IsComplete()
        {
            Settings.gameManager.currentPlayer.MakeAllResourceCardUsable();

            return true;
        }

        public override void OnEndPhase()
        {
        }

        public override void OnStartPhase()
        {
        }
    }

}