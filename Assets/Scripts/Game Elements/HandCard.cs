using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Legendary.GameElements
{
    [CreateAssetMenu(menuName = "Game Elements/My Hand Card")]
    public class HandCard : GE_Logic
    {
        public SO.GameEvent onCurrentCardSelected;
        public Legendary.GameStates.GameState holdingCard;

        public CardVariable currentCard;

        public override void OnClick(CardInstance inst)
        {
            currentCard.Set(inst);
            Settings.gameManager.SetState(holdingCard);
            onCurrentCardSelected.Raise();
        }

        public override void OnHighligth(CardInstance inst)
        {

        }
    }
}