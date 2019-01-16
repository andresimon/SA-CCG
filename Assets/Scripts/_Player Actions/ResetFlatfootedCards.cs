using UnityEngine;
using System.Collections;

namespace Legendary
{
    [CreateAssetMenu(menuName = "Actions/Player Actions/Reset Flat Footed")]
    public class ResetFlatfootedCards : PlayerAction
    {
        public override void Execute(PlayerHolder player)
        {
            foreach (CardInstance c in player.downCards)
            {
                if ( c.isFlatfooted )
                {
                    c.SetFlatfooted(false);
                }
            }
        }
    }

}