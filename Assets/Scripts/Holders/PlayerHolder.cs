using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Legendary
{

    [CreateAssetMenu(menuName = "Holders/Player Holder")]
    public class PlayerHolder : ScriptableObject
    {
        public string[] startingCards;
        public SO.TransformVariable handGrid;
        public SO.TransformVariable downGrid;

        public GameElements.GE_Logic handLogic;
        public GameElements.GE_Logic downLogic;

        [System.NonSerialized] public List<CardInstance> handCards = new List<CardInstance>();
        [System.NonSerialized] public List<CardInstance> downCards = new List<CardInstance>();
    }

}
