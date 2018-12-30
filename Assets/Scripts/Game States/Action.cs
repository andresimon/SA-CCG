using UnityEngine;
using System.Collections;

namespace Legendary.GameStates
{
    public abstract class Action : ScriptableObject
    {
        public abstract void Execute(float d);
    }
}