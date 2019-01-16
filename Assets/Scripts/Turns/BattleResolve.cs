using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Legendary
{
    [CreateAssetMenu(menuName = "Turns/Battle Resolve")]
    public class BattleResolve : Phase
    {
        public Element attackElement;
        public Element defenseElement;

        public override bool IsComplete()
        {
            PlayerHolder p = Settings.gameManager.currentPlayer;
            PlayerHolder e = Settings.gameManager.GetEnemyOf(p);

            if (p.attackingCards.Count == 0)
                return true;

            Dictionary<CardInstance, BlockInstance> blockDict = Settings.gameManager.GetBlockInstances();

            for (int i = 0; i < p.attackingCards.Count; i++)
            {
                CardInstance inst = p.attackingCards[i];
                Card c = inst.viz.card;
                CardProperties attack = c.GetProperty(attackElement);
                if (attack == null)
                {
                    Debug.LogError("You are attacking with a card that can't attack");
                    continue;
                }

                int damageValue = attack.intValue;

                BlockInstance bi = GetBlockInstanceOfAttacker(inst, blockDict);
                if ( bi != null )
                {
                    for (int b = 0; b < bi.blocker.Count; b++)
                    {
                        CardProperties def = c.GetProperty(defenseElement);
                        if ( def == null )
                        {
                            Debug.LogWarning("You are trying to block with a card with no defense element!");
                            continue;
                        }

                        damageValue -= def.intValue;

                        if ( def.intValue <= damageValue )
                        {
                            bi.blocker[b].CardInstanceToGraveyard();
                        }
                    }
                }

                if (damageValue <= 0)
                {
                    damageValue = 0;
                    inst.CardInstanceToGraveyard();
                }

                p.DropCard(inst, false);
                p.currentHolder.SetCardsOffBatlleLine(inst);
                inst.SetFlatfooted(true);
                e.DoDamage(damageValue);
            }

            Settings.gameManager.ClearBlockInstances();
            p.attackingCards.Clear();

            return true;

        }

        BlockInstance GetBlockInstanceOfAttacker(CardInstance attacker, Dictionary<CardInstance, BlockInstance> blockInstances)
        {
            BlockInstance r = null;
            blockInstances.TryGetValue(attacker, out r);

            return r;
        }

        public override void OnEndPhase()
        {
        }

        public override void OnStartPhase()
        {
            
        }
    }

}
