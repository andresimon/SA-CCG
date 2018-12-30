using UnityEngine;
using System.Collections;

namespace Legendary
{
    [CreateAssetMenu(menuName = "Card Type/Master Strike")]
    public class MasterStrikeCard : CardType
    {
        public override void OnSetType(CardViz viz)
        {
            base.OnSetType(viz);

            int qtd = viz.HeroPropertiesHolder.Length;
            for (int i = 0; i < qtd; i++)
            {
                viz.HeroPropertiesHolder[i].SetActive(false);
            }
        }

    }
}