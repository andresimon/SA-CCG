using UnityEngine;
using System.Collections;

namespace Legendary
{
    [CreateAssetMenu(menuName = "Holders/Card Holder")]
    public class CardHolders : ScriptableObject
    {
        public SO.TransformVariable handGrid;
        public SO.TransformVariable resourcesGrid;
        public SO.TransformVariable downGrid;
        public SO.TransformVariable battleLine;

        public void SetCardsOnBatlleLine(CardInstance card)
        {
            Vector3 position = card.viz.gameObject.transform.position;
            Settings.SetParentForCard(card.viz.gameObject.transform, battleLine.value.transform);
            position.z = card.viz.gameObject.transform.position.z;
            position.y = card.viz.gameObject.transform.position.y;
            card.viz.gameObject.transform.position = position;
        }

        public void LoadPlayer(PlayerHolder p)
        {
            foreach (CardInstance c in p.downCards)
            {
                Settings.SetParentForCard(c.viz.gameObject.transform, downGrid.value.transform);
            }

            foreach (CardInstance c in p.handCards)
            {
                Settings.SetParentForCard(c.viz.gameObject.transform, handGrid.value.transform);
            }

            foreach (ResourceHolder c in p.resourcesList)
            {
                Settings.SetParentForCard(c.cardObj.transform, resourcesGrid.value.transform);
            }
        }

    }

}
