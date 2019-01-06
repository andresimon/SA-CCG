using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Legendary
{
    public class CardInstance : MonoBehaviour, IClickable
    {
        public CardViz viz;
        public GameElements.GE_Logic currentLogic;

        void Start()
        {
            viz = GetComponent<CardViz>();
        }

        public void OnClick()
        {
            if (currentLogic == null) return;

            currentLogic.OnClick(this);
        }

        public void OnHighligth()
        {
            if (currentLogic == null) return;

            currentLogic.OnHighligth(this);
        }
    }
}