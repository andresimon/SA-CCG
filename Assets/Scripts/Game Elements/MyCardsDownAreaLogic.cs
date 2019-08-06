using UnityEngine;

namespace Legendary.GameElements
{
    [CreateAssetMenu(menuName = "Areas/MyCardsDownWhenHoldingCard")]
    public class MyCardsDownAreaLogic : AreaLogic
    {
        public CardVariable card;
        public CardType creatureType;
        public CardType bystanderType;

        public override void Execute()
        {
            if (card.value == null) return;

            Card c = card.value.viz.card;

            if (c.cardType == creatureType)
            {
                MultiplayerManager.singleton.PlayerWantsToUseCard(c.instID, GameManager.singleton.localPlayer.photonId, MultiplayerManager.CardOpertation.dropCreatureCard);
            }
            else
            if (c.cardType == bystanderType)
            {
                MultiplayerManager.singleton.PlayerWantsToUseCard(c.instID, GameManager.singleton.localPlayer.photonId, MultiplayerManager.CardOpertation.dropResourcesCard);
            }
        }
    }

}