using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Legendary
{

    [CreateAssetMenu(menuName = "Card")]
    public class Card : ScriptableObject
    {
        public CardType cardType;
        public CardProperties[] properties; 
    }
}
