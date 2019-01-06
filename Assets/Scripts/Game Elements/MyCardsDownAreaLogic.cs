using UnityEngine;
using System.Collections;

namespace Legendary.GameElements
{
    [CreateAssetMenu(menuName = "Areas/MyCardsDownWhenHoldingCard")]
    public class MyCardsDownAreaLogic : AreaLogic
    {
        public CardVariable card;
        public CardType creatureType;
        public SO.TransformVariable areaGrid;
        public GameElements.GE_Logic cardDownLogic;

        public override void Execute()
        {
            if (card.value == null) return;

            if (card.value.viz.card.cardType == creatureType)
            {
                Settings.SetParentForCard(card.value.transform, areaGrid.value.transform);
                card.value.gameObject.SetActive(true);
                card.value.currentLogic = cardDownLogic;
            }
        }
    }

}