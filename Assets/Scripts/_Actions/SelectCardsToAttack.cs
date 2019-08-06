using Legendary.GameStates;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Legendary
{

    [CreateAssetMenu(menuName = "Actions/SelectCardsToAttack")]
    public class SelectCardsToAttack : Action
    {
        public override void Execute(float d)
        {
            if (Input.GetMouseButtonDown(0))
            {
                List<RaycastResult> results = Settings.GetUIObjs();

                foreach (RaycastResult r in results)
                {
                    CardInstance inst = r.gameObject.GetComponentInParent<CardInstance>();
                    PlayerHolder p = Settings.gameManager.currentPlayer;

                    if (!p.downCards.Contains(inst)) return;

                    MultiplayerManager.singleton.PlayerWantsToUseCard(inst.viz.card.instID, p.photonId, MultiplayerManager.CardOpertation.setCardForBattle);

                    //if ( inst.CanAttack())
                    //{
                    //    p.attackingCards.Add(inst);
                    //    p.currentHolder.SetCardsOnBatlleLine(inst);
                    //}

                }

            }
        }
    }

}