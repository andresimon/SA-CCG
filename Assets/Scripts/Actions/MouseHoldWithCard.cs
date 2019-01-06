using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Legendary.GameStates
{

    [CreateAssetMenu(menuName = "Actions/MouseHoldWithCard")]
    public class MouseHoldWithCard : Action
    {
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
                    // Check for dropable areas
                }

                Settings.gameManager.SetState(playerControlState);
                onPlayerControlState.Raise();

                return;
            }
        }

    }

}