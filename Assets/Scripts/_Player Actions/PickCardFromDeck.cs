using UnityEngine;

namespace Legendary
{
    [CreateAssetMenu(menuName = "Actions/Player Actions/PickCardFromDeck")]
    public class PickCardFromDeck : PlayerAction
    {
        public override void Execute(PlayerHolder player)
        {
            GameManager.singleton.PickNewCardFromDeck(player);
        }
    }

}