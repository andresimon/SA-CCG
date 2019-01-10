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

        public Turn[] turns;
        public int turnIndex;

        public SO.GameEvent onTurnChanged;
        public SO.GameEvent onPhaseCompleted;
        public SO.StringVariable turnText;

        private void Start()
        {
            Settings.gameManager = this;
            CreateStartingCards();

            turnText.value = turns[turnIndex].player.userName;
            onTurnChanged.Raise();

        }

        private void Update()
        {
            bool isComplete = turns[turnIndex].Execute();
            if ( isComplete )
            {
                turnIndex++;
                if ( turnIndex > turns.Length - 1)
                {
                    turnIndex = 0;
                }

                turnText.value = turns[turnIndex].player.userName;
                onTurnChanged.Raise();
            }

            if ( currentState != null)
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