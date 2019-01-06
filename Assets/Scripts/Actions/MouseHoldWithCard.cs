using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Legendary.GameStates
{

    [CreateAssetMenu(menuName = "Actions/MouseHoldWithCard")]
    public class MouseHoldWithCard : Action
    {
        public CardVariable currentCard;
        public GameState playerControlState;
        public SO.GameEvent onPlayerControlState;

        public override void Execute(float d)
        {
            bool mouseIsDown = Input.GetMouseButton(0);

            if ( !mouseIsDown )
            {
                List<RaycastResult> results = Settings.GetUIObjs();

                foreach (RaycastResult r in results)
                {
                    GameElements.Area a = r.gameObject.GetComponentInParent<GameElements.Area>();
                    if ( a != null )
                    {
                        a.OnDrop();
                        break;
                    }
                }

                currentCard.value.gameObject.SetActive(true);
                currentCard.value = null;

                Settings.gameManager.SetState(playerControlState);
                onPlayerControlState.Raise();

                return;
            }
        }

    }

}