using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Legendary
{
    public class CardInstance : MonoBehaviour, IClickable
    {
        public CardViz viz;
        public GameElements.GE_Logic currentLogic;
        public bool isFlatfooted;

        void Start()
        {
            viz = GetComponent<CardViz>();
        }

        public virtual bool CanAttack()
        {
            bool result = true;

            if (isFlatfooted) result = false;

            if ( viz.card.cardType.TypeAllowsForAttack(this))
            {
                result = true;
            }

            return result;
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