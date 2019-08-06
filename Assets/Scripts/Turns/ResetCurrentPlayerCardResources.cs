using UnityEngine;
using System.Collections;

namespace Legendary
{
    [CreateAssetMenu(menuName = "Turns/ResetCurrentPlayerCardResources")]
    public class ResetCurrentPlayerCardResources : Phase
    {
        public override bool IsComplete()
        {
           // MultiplayerManager.singleton.PlayerWantsToResetResourcesCards(player.photonId);

            return true;
        }

        public override void OnStartPhase()
        {
        }
    }

}