using UnityEngine;
using System.Collections;

namespace Legendary.GameElements
{
    [CreateAssetMenu(menuName = "Game Elements/My Card Down")]
    public class MyCardDown : GE_Logic
    {
        public override void OnClick(CardInstance inst)
        {
            Debug.Log("This card is my, but itson the table.");
        }

        public override void OnHighligth(CardInstance inst)
        {

        }
    }
}