using UnityEngine;

namespace Legendary
{

    public abstract class Condition : ScriptableObject
    {
        public abstract bool IsValid();
    }

}