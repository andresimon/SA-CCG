using UnityEngine;

namespace Legendary
{

    [CreateAssetMenu(menuName = "Turns/Turn")]
    public class Turn : ScriptableObject
    {
        public PlayerHolder player;

        [System.NonSerialized] public int index = 0;
        public Phase[] phases;
        public PhaseVariable currentPhase;

        public PlayerAction[] turnStartActions;

        public void OnTurnStart()
        {
            if (turnStartActions == null) return;

            for (int i = 0; i < turnStartActions.Length; i++)
            {
                turnStartActions[i].Execute(player);
            }

            MultiplayerManager.singleton.SendPhase(phases[index].name, player.name);
        }

        public bool Execute()
        {
            bool result = false;

            //currentPhase.value = phases[index];
            //phases[index].OnStartPhase();

            //bool phaseIsComplete = phases[index].IsComplete();

            //if ( phaseIsComplete )
            //{
            //    phases[index].OnEndPhase();

            //    index++;
            //    if ( index > phases.Length - 1)
            //    {
            //        index = 0;
            //        result = true;
            //    }

            //    MultiplayerManager.singleton.SendPhase(phases[index].name, player.name);
            //}
            return result;
        }

        public void EndCurrentPhase()
        {
            phases[index].forceExit = true;
        }
    }

}