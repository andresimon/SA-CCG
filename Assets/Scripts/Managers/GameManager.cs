using UnityEngine;
using Legendary.GameStates;
using System.Collections.Generic;

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

        public PlayerStatsUI[] statsUIs;

        Dictionary<CardInstance, BlockInstance> blockInstances = new Dictionary<CardInstance, BlockInstance>();

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

            turns[0].OnTurnStart();
            turnText.value = turns[turnIndex].player.userName;
            onTurnChanged.Raise();
        }

        public void LoadPlayerOnActive(PlayerHolder p)
        {
            PlayerHolder prevPlayer = playerOneHolder.playerHolder;
            LoadPlayerOnHolder(prevPlayer, otherPlayersHolder, statsUIs[1]);
            LoadPlayerOnHolder(p, playerOneHolder, statsUIs[0]);
        }

        public void LoadPlayerOnHolder(PlayerHolder p, CardHolders h, PlayerStatsUI ui)
        {
            h.LoadPlayer(p, ui);
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
            ResourcesManager rm = Settings.GetResourcesManager();

            for (int i = 0; i < all_Players.Length; i++)
            {
                all_Players[i].Init();

                if ( i == 0 )
                {
                    all_Players[i].currentHolder = playerOneHolder;
                }
                else
                {
                    all_Players[i].currentHolder = otherPlayersHolder;
                }

                all_Players[i].statsUI = statsUIs[i];
                all_Players[i].currentHolder.LoadPlayer(all_Players[i], all_Players[i].statsUI);
            }
        }

        public void PickNewCardFromDeck(PlayerHolder p)
        {
            if ( p.all_Cards.Count == 0 )
            {
                Debug.Log("Game Over");
                return;
            }

            ResourcesManager rm = Settings.GetResourcesManager();

            string cardId = p.all_Cards[0];
            p.all_Cards.RemoveAt(0);

            GameObject go = Instantiate(cardPrefab) as GameObject;
            CardViz v = go.GetComponent<CardViz>();
            v.LoadCard(rm.GetCardInstance(cardId));
            CardInstance inst = go.GetComponent<CardInstance>();
            inst.owner = p;
            inst.currentLogic = p.handLogic;
            Settings.SetParentForCard(go.transform, p.currentHolder.handGrid.value);
            p.handCards.Add(inst);
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

        public PlayerHolder GetEnemyOf(PlayerHolder p)
        {
            for (int i = 0; i < all_Players.Length; i++)
            {
                if ( all_Players[i] != p )
                {
                    return all_Players[i];
                }
            }
            return null;
        }

        public Dictionary<CardInstance, BlockInstance> GetBlockInstances()
        {
            return blockInstances;
        }

        public void ClearBlockInstances()
        {
            blockInstances.Clear();
        }

        BlockInstance GetBlockInstanceOfAttacker(CardInstance attacker)
        {
            BlockInstance r = null;
            blockInstances.TryGetValue(attacker, out r);

            return r;
        }

        public void AddBlockInstance(CardInstance attacker, CardInstance blocker)
        {
            BlockInstance b = null;
            b = GetBlockInstanceOfAttacker(attacker);
            if ( b == null )
            {
                b = new BlockInstance();
                b.attacker = attacker;
                blockInstances.Add(attacker, b);
            }

            if ( !b.blocker.Contains(blocker))
                b.blocker.Add(blocker);
        }
    }
}