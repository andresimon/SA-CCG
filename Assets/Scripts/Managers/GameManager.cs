using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Legendary.GameStates;

namespace Legendary
{
    public class GameManager : MonoBehaviour
    {
        public PlayerHolder currentPlayer;
        public GameState currentState;
        public GameObject cardPrefab;

        private void Start()
        {
            Settings.gameManager = this;
            CreateStartingCards();
        }

        private void Update()
        {
            currentState.Tick(Time.deltaTime);
        }

        void CreateStartingCards()
        {
            ResourcesManager rm = Settings.GetResourcesManager();

            for (int i = 0; i < currentPlayer.startingCards.Length; i++)
            {
                GameObject go = Instantiate(cardPrefab) as GameObject;
                CardViz v = go.GetComponent<CardViz>();
                v.LoadCard(rm.GetCardInstance(currentPlayer.startingCards[i]));
                CardInstance inst = go.GetComponent<CardInstance>();
                inst.currentLogic = currentPlayer.handLogic;

                Settings.SetParentForCard(go.transform, currentPlayer.handGrid.value);
            }
        }

        public void SetState(GameState state)
        {
            currentState = state;
        }

    }
}