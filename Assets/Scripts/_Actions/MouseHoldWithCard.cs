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

        public GameState playerBlockState;

        public Phase blockPhase;

        public bool isBlocking;

        public override void Execute(float d)
        {
            bool mouseIsDown = Input.GetMouseButton(0);

            if ( !mouseIsDown )
            {
                GameManager gm = Settings.gameManager;

                List<RaycastResult> results = Settings.GetUIObjs();

                if (!isBlocking)
                {
                    foreach (RaycastResult r in results)
                    {
                        GameElements.Area a = r.gameObject.GetComponentInParent<GameElements.Area>();
                        if (a != null)
                        {
                            a.OnDrop();
                            break;
                        }
                    }

                    currentCard.value.gameObject.SetActive(true);
                    currentCard.value = null;

                    Settings.gameManager.SetState(playerControlState);
                    onPlayerControlState.Raise();

                }
                else
                {
                    foreach (RaycastResult r in results)
                    {
                        CardInstance c = r.gameObject.GetComponentInParent<CardInstance>();
                        if ( c != null )
                        {
                            int count = 0;
                            bool block = c.CanBeBlocked(currentCard.value, ref count);

                            if (block)
                            {
                                CardInstance myCard = currentCard.value;
                                MultiplayerManager.singleton.PlayerBlocksTargetCard(myCard.viz.card.instID, myCard.owner.photonId, c.viz.card.instID, c.owner.photonId);
                                //Settings.SetCardForBlock(currentCard.value.transform, c.transform, count);
                            }

                            currentCard.value.gameObject.SetActive(true);
                            currentCard.value = null;
                            onPlayerControlState.Raise();
                            Settings.gameManager.SetState(playerBlockState);

                            break;
                        }
                    }
                }
                return;
            }
        }

    }

}