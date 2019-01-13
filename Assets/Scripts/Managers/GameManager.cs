using UnityEngine;
using Legendary.GameStates;

namespace Legendary
{
    public class GameManager : MonoBehaviour
    {
        [System.NonSerialized] public PlayerHolder[] all_Players;
        public PlayerHolder currentPlayer;

        public CardHolders playerOneHolder;
        public CardHolders otherPlayersHolder;

        public GameState currentState;
        public GameObject cardPrefab;

        public Turn[] turns;
        public int turnIndex;

        public SO.GameEvent onTurnChanged;
        public SO.GameEvent onPhaseCompleted;
        public SO.StringVariable turnText;

        public static GameManager singleton;

        private void Awake()
        {
            singleton = this;

            all_Players = new PlayerHolder[turns.Length];
            for (int i = 0; i < turns.Length; i++)
            {
                all_Players[i] = turns[i].player;
            }

            currentPlayer = turns[0].player;
        }

        private void Start()
        {
            Settings.gameManager = this;

            SetupPlayers();

            CreateStartingCards();

            turnText.value = turns[turnIndex].player.userName;
            onTurnChanged.Raise();
        }

        public bool switchPlayer;

        private void Update()
        {
            if ( switchPlayer )
            {
                switchPlayer = false;

                playerOneHolder.LoadPlayer(all_Players[0]);
                otherPlayersHolder.LoadPlayer(all_Players[1]);
            }

            bool isComplete = turns[turnIndex].Execute();
            if ( isComplete )
            {
                turnIndex++;
                if ( turnIndex > turns.Length - 1)
                {
                    turnIndex = 0;
                }

                // The current player has changed here
                currentPlayer = turns[turnIndex].player;
                turns[turnIndex].OnTurnStart();
                turnText.value = turns[turnIndex].player.userName;
                onTurnChanged.Raise();
            }

            if ( currentState != null)
                currentState.Tick(Time.deltaTime);
        }

        void SetupPlayers()
        {
            foreach (PlayerHolder p in all_Players)
            {
                if ( p.isHumanPlayer )
                {
                    p.currentHolder = playerOneHolder;
                }
                else
                {
                    p.currentHolder = otherPlayersHolder;
                }
            }
        }

        void CreateStartingCards()
        {
            ResourcesManager rm = Settings.GetResourcesManager();

            for (int p = 0; p < all_Players.Length; p++)
            {
                for (int i = 0; i < all_Players[p].startingCards.Length; i++)
                {
                    GameObject go = Instantiate(cardPrefab) as GameObject;
                    CardViz v = go.GetComponent<CardViz>();
                    v.LoadCard(rm.GetCardInstance(all_Players[p].startingCards[i]));
                    CardInstance inst = go.GetComponent<CardInstance>();
                    inst.currentLogic = all_Players[p].handLogic;

                    Settings.SetParentForCard(go.transform, all_Players[p].currentHolder.handGrid.value);

                    all_Players[p].handCards.Add(inst);
                }

                Settings.RegisterEvent("Created cards for player " + all_Players[p].userName, all_Players[p].playerColor);
            }
        }

        public void SetState(GameState state)
        {
            currentState = state;
        }

        public void EndCurrentPhase()
        {
            Settings.RegisterEvent(turns[turnIndex].name + " finished", currentPlayer.playerColor);

            turns[turnIndex].EndCurrentPhase();
        }


    }
}