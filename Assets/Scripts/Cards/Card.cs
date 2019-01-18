using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Legendary
{

    [CreateAssetMenu(menuName = "Card")]
    public class Card : ScriptableObject
    {
        [System.NonSerialized] public int instID;
        [System.NonSerialized] public CardViz cardViz;

        public CardType cardType;
        public int cost;
        public CardProperties[] properties; 

        public CardProperties GetProperty(Element e)
        {
            for (int i = 0; i < properties.Length; i++)
            {
                if ( properties[i].element == e )
                {
                    return properties[i];
                }
            }
            return null;
        }
    }
}
