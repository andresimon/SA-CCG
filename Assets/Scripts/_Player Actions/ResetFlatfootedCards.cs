using UnityEngine;
using System.Collections;

namespace Legendary
{
    [CreateAssetMenu(menuName = "Actions/Player Actions/Reset Flat Footed")]
    public class ResetFlatfootedCards : PlayerAction
    {
        public override void Execute(PlayerHolder player)
        {
            MultiplayerManager.singleton.PlayerWantsToResetResourcesCards(player.photonId);
            MultiplayerManager.singleton.PlayerWantsToResetFlatfootedCards(player.photonId);
        }
    }

}