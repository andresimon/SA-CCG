using UnityEngine;

namespace Legendary
{

    [CreateAssetMenu(menuName = "Variables/Card Variable")]
    public class CardVariable : ScriptableObject
    {
        public CardInstance value;

        public void Set(CardInstance v)
        {
            value = v;
        }
    }

}