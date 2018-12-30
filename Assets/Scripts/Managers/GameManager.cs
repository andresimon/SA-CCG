using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Legendary.GameStates;

namespace Legendary
{
    public class GameManager : MonoBehaviour
    {
        public GameState currentState;

        void Update()
        {
            currentState.Tick(Time.deltaTime);
        }
    }
}