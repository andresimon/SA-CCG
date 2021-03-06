﻿using UnityEngine;
using Legendary.GameStates;
using System.Collections.Generic;

namespace Legendary
{
    public class GameManager : MonoBehaviour
    {
        public ResourcesManager resourcesManager;

        public bool isMultiplayer;

        [System.NonSerialized] public PlayerHolder[] all_Players;
        public PlayerHolder currentPlayer;

        public PlayerHolder localPlayer;
        public PlayerHolder clientPlayer;

        public CardHolders playerOneHolder;
        public CardHolders otherPlayersHolder;

        public GameState currentState;
        public GameObject cardPrefab;

        //public Turn[] turns;
        //public int turnIndex;

        public SO.GameEvent onTurnChanged;
        public SO.GameEvent onPhaseCompleted;
        //public SO.StringVariable turnText;

        Phase currentPhase;

        public PlayerStatsUI[] statsUIs;

        public SO.TransformVariable graveyardVariable;
        List<CardInstance> graveyardCards = new List<CardInstance>();

        public Element defenceProperty;

        bool isInit;

        Dictionary<CardInstance, BlockInstance> blockInstances = new Dictionary<CardInstance, BlockInstance>();

        public static GameManager singleton;

        private void Awake()
        {
            Settings.gameManager = this;
            singleton = this;
        }

        public void InitGame()
        {
            //all_Players = new PlayerHolder[turns.Length];
            //Turn[] _turns = new Turn[2];

            //for (int i = 0; i < turns.Length; i++)
            //{
            //    all_Players[i] = turns[i].player;

            //    if ( all_Players[i].photonId == startingPlayer )
            //    {
            //        _turns[0] = turns[i];
            //    }
            //    else
            //    {
            //        _turns[1] = turns[i];
            //    }
            //}
            //turns = _turns;

            //SetupPlayers();

            //turnText.value = turns[turnIndex].player.userName;
            //onTurnChanged.Raise();
            //turns[0].OnTurnStart();

            isInit = true;
        }

        public void LoadPlayerOnActive(PlayerHolder p)
        {
            PlayerHolder prevPlayer = playerOneHolder.playerHolder;

            if ( prevPlayer != p)
                LoadPlayerOnHolder(prevPlayer, otherPlayersHolder, statsUIs[1]);

            LoadPlayerOnHolder(p, playerOneHolder, statsUIs[0]);
        }

        public void LoadPlayerOnHolder(PlayerHolder p, CardHolders h, PlayerStatsUI ui)
        {
            h.LoadPlayer(p, ui);
        }

        public void SetCurrentPhase(Phase phase)
        {
            currentPhase = phase;
        }

        private void Update()
        {
            if (!isInit) return;

            if (currentPhase == null) return;

            bool phaseIsComplete = currentPhase.IsComplete();

            if (phaseIsComplete)
            {
                currentPhase = null;
               MultiplayerManager.singleton.PlayerEndsPhase(localPlayer.photonId);
            } 

            //bool isComplete = turns[turnIndex].Execute();

            //if (!isMultiplayer)
            //{
            //    if (isComplete)
            //    {
            //        turnIndex++;
            //        if (turnIndex > turns.Length - 1)
            //        {
            //            turnIndex = 0;
            //        }

            //        // The current player has changed here
            //        currentPlayer = turns[turnIndex].player;
            //        turns[turnIndex].OnTurnStart();
            //        turnText.value = turns[turnIndex].player.userName;
            //        onTurnChanged.Raise();
            //    }
            //}
            //else
            //{
            //    if (isComplete)
            //    {
            //        MultiplayerManager.singleton.PlayerEndsTurn(currentPlayer.photonId);
            //    }
            //}

            //if ( currentState != null)
            //    currentState.Tick(Time.deltaTime);
        }

        //public int GetNextPlayerId()
        //{
        //    int result = turnIndex;

        //    result++;
        //    if (result > turns.Length - 1)
        //        result = 0;

        //    return turns[result].player.photonId;
        //}

        //int GetPlayerTurnIndex(int photonId)
        //{
        //    for (int i = 0; i < turns.Length; i++)
        //    {
        //        if (turns[i].player.photonId == photonId)
        //            return i;
        //    }

        //    return -1;
        //}

        //public void ChangeCurrentTurn(int photonId)
        //{
        //    turnIndex = GetPlayerTurnIndex(photonId);
        //    currentPlayer = turns[turnIndex].player;
        //    turns[turnIndex].OnTurnStart();
        //    turnText.value = turns[turnIndex].player.userName;
        //    onTurnChanged.Raise();
        //}

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
            MultiplayerManager.singleton.PlayerPicksCardFromDeck(p);
        }

        public void SetState(GameState state)
        {
            currentState = state;
        }

        public void EndCurrentPhase()
        {
            if (currentPhase != null)
            {
                MultiplayerManager.singleton.PlayerEndsPhase(localPlayer.photonId);
                //turns[turnIndex].EndCurrentPhase();
            }
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

        public void AddBlockInstance(CardInstance attacker, CardInstance blocker, ref int count)
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

            count = b.blocker.Count;
        }

        public void PutCardOnGraveyard(CardInstance c)
        {
            c.owner.CardToGraveyard(c);
            graveyardCards.Add(c);

            c.transform.SetParent(graveyardVariable.value);
            Vector3 p = Vector3.zero;
            p.x -= graveyardCards.Count * 5;
            p.z = graveyardCards.Count * 5;

            c.transform.localPosition = p;
            c.transform.localRotation = Quaternion.identity;
            c.transform.localScale = Vector3.one;
        }

        public void LocalPlayerEndsBattleResolve()
        {
           // turns[turnIndex].EndCurrentPhase();
        }

    }
}