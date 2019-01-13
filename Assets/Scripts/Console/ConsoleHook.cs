﻿using UnityEngine;
using System.Collections;

namespace Legendary
{
    [CreateAssetMenu(menuName = "Console/Hook")]
    public class ConsoleHook : ScriptableObject
    {

        [System.NonSerialized] public ConsoleManager consoleManager;

        public void RegisterEvent(string s, Color color)
        {
            consoleManager.RegisterEvent(s, color);
        }
    }

}