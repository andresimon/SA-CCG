using UnityEngine;

namespace Legendary.GameElements
{
    [CreateAssetMenu(menuName = "Areas/MyCardsDownWhenHoldingCard")]
    public class MyCardsDownAreaLogic : AreaLogic
    {
        public CardVariable card;
        public CardType creatureType;
        public CardType bystanderType;
        public SO.TransformVariable areaGrid;
        public SO.TransformVariable resourcesGrid;
        public GameElements.GE_Logic cardDownLogic;

        public override void Execute()
        {
            if (card.value == null) return;

            Card c = card.value.viz.card;

            if (c.cardType == creatureType)
            {
                bool canUse = Settings.gameManager.currentPlayer.CanUseCard(c);

                if (canUse)
                {
                    Settings.DropHeroCard(card.value.transform, areaGrid.value.transform, card.value);
                    card.value.currentLogic = cardDownLogic;
                }
                else
                {
                    Settings.RegisterEvent("Not enough resources to use card", Color.red);

                }
                card.value.gameObject.SetActive(true);
            }
            else
            if (c.cardType == bystanderType)
            {
                bool canUse = Settings.gameManager.currentPlayer.CanUseCard(c);

                if (canUse)
                {
                    Settings.SetParentForCard(card.value.transform, resourcesGrid.value.transform);
                    card.value.currentLogic = cardDownLogic;

                    Settings.gameManager.currentPlayer.AddResourceCard(card.value.gameObject);
                }
                else
                {
                    int limit = Settings.gameManager.currentPlayer.resourcesPerTurn;
                    Settings.RegisterEvent("Can't drop more than " + limit + " resource card per turn",Color.red);

                }
                card.value.gameObject.SetActive(true);
            }
        }
    }

}