using UnityEngine;

namespace Legendary
{
    public class CardInstance : MonoBehaviour, IClickable
    {
        public PlayerHolder owner;

        public CardViz viz;
        public GameElements.GE_Logic currentLogic;
        public bool isFlatfooted;

        void Start()
        {
            viz = GetComponent<CardViz>();
        }

        public bool CanBeBlocked(CardInstance block, ref int count)
        {
            bool result = owner.attackingCards.Contains(this);

            if ( result && viz.card.cardType.canAttack )
            {
                result = true;

                // if a card has flying than can be blocked by non flying , you can check it here, or cases like that

                if ( result )
                    Settings.gameManager.AddBlockInstance(this, block, ref count);
            }
            else
            {
                result = false;
            }

            return result;
        }
        public void SetFlatfooted(bool isFlat)
        {
            isFlatfooted = isFlat;

            if ( isFlatfooted )
                transform.localEulerAngles = new Vector3(0, 0, 90);
            else
                transform.localEulerAngles = Vector3.zero;

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

        public void CardInstanceToGraveyard()
        {
            Settings.gameManager.PutCardOnGraveyard(this);
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