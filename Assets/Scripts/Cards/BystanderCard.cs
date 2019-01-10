using UnityEngine;

namespace Legendary
{
    [CreateAssetMenu(menuName = "Card Type/Bystander")]
    public class BystanderCard : CardType
    {
        public override void OnSetType(CardViz viz)
        {
            base.OnSetType(viz);

        }

    }
}