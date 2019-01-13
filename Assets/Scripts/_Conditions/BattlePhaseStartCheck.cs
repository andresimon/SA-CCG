using UnityEngine;
using System.Collections;
using Legendary.GameStates;

namespace Legendary
{

    [CreateAssetMenu(menuName = "Actions/BattlePhaseStartCheck")]
    public class BattlePhaseStartCheck : Condition
    {

        public override bool IsValid()
        {
            GameManager gm = GameManager.singleton;
            PlayerHolder p = gm.currentPlayer;

            int count = p.downCards.Count;
            for (int i = 0; i < p.downCards.Count; i++)
            {
                if (p.downCards[i].isFlatfooted) count--;
            }

            return (count > 0);
        }
    }

}
