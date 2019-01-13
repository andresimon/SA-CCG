using UnityEngine;

namespace Legendary
{

    public abstract class PlayerAction : ScriptableObject
    {
        public abstract void Execute(PlayerHolder player);
    }

}