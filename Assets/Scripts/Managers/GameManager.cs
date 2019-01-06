using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Legendary.GameStates;

namespace Legendary
{
    public class GameManager : MonoBehaviour
    {
        public GameState currentState;

        private void Start()
        {
            Settings.gameManager = this;
        }

        private void Update()
        {
            currentState.Tick(Time.deltaTime);
        }

        public void SetState(GameState state)
        {
            currentState = state;
        }

    }
}