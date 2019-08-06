using UnityEngine;
using System.Collections.Generic;

namespace Legendary
{

    public class MultiplayerManager : Photon.MonoBehaviour
    {
        #region Variables

        public static MultiplayerManager singleton;

        Transform multiplayerReferences;
        public MainDataHolder dataHolder;

        //public PlayerHolder localPlayerHolder;
        //public PlayerHolder clientPlayerHolder;

        bool gameStarted;
        public bool countPlayers;

        GameManager gm
        {
            get { return GameManager.singleton; }
        }

        public TurnsHolder turnsHolder;

        int playerIndex;
        int phaseIndex;
        int currentPlayerId;

        #endregion Variables

        #region Player Management

        List<NetworkPrint> players = new List<NetworkPrint>();
        NetworkPrint localPlayer;

        NetworkPrint GetPlayer(int photonId)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].photonID == photonId)
                    return players[i];
            }

            return null;
        }

        #endregion Player Management

        #region Tick

        private void Update()
        {
            if (!gameStarted && countPlayers)
            {
                if (players.Count > 1)
                {
                    gameStarted = true;
                    Invoke("StartMatch", 0.2f);
                }
            }
        }

        #endregion Tick

        #region Init
        void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            turnsHolder.Init();

            multiplayerReferences = new GameObject("references").transform;
            DontDestroyOnLoad(multiplayerReferences.gameObject);

            singleton = this;
            DontDestroyOnLoad(this.gameObject);

            InstantiateNetworkPrint();
            NetworkManager.singleton.LoadGameScene();
        }

        void InstantiateNetworkPrint()
        {
            PlayerProfile profile = Resources.Load("PlayerProfile") as PlayerProfile;

            object[] data = new object[1];
            data[0] = profile.cardsIDs;

            PhotonNetwork.Instantiate("NetworkPrint", Vector3.zero, Quaternion.identity, 0, data);
        }
        #endregion Init

        #region Starting the Match

        public void StartMatch()
        {
            ResourcesManager rm = gm.resourcesManager;

            if (NetworkManager.isMaster)
            {
                List<int> playerId = new List<int>();
                List<int> cardInstId = new List<int>();
                List<string> cardName = new List<string>();

                foreach (NetworkPrint p in players)
                {
                    foreach (string id in p.GetStartingCardsIDs())
                    {
                        Card card = rm.GetCardInstance(id);
                        playerId.Add(p.photonID);
                        cardInstId.Add(card.instID);
                        cardName.Add(id);

                        if (p.isLocal)
                        {
                            p.playerHolder = gm.localPlayer;
                            p.playerHolder.photonId = p.photonID;
                        }
                        else
                        {
                            p.playerHolder = gm.clientPlayer;
                            p.playerHolder.photonId = p.photonID;
                        }
                    }
                }

                for (int i = 0; i < playerId.Count; i++)
                {
                    photonView.RPC("RPC_PlayerCreatesCard", PhotonTargets.All, playerId[i], cardInstId[i], cardName[i]);
                }

                photonView.RPC("RPC_InitGame", PhotonTargets.All, 1);
                photonView.RPC("RPC_SetPhaseToPlayer", PhotonTargets.All, 1, turnsHolder.phaseOrder[phaseIndex]);
            }
            else
            {
                foreach (NetworkPrint p in players)
                {
                    if (p.isLocal)
                    {
                        p.playerHolder = gm.localPlayer;
                        p.playerHolder.photonId = p.photonID;
                    }
                    else
                    {
                        p.playerHolder = gm.clientPlayer;
                        p.playerHolder.photonId = p.photonID;
                    }
                }
            }
        }

        [PunRPC]
        public void RPC_PlayerCreatesCard(int photonId, int instId, string cardName)
        {
            Card c = gm.resourcesManager.GetCardInstance(cardName);
            c.instID = instId;

            NetworkPrint p = GetPlayer(photonId);
            p.AddCard(c);
        }

        [PunRPC]
        public void RPC_InitGame(int startingPlayer)
        {
            gm.isMultiplayer = true;
            gm.InitGame();
        }

        public void AddPlayer(NetworkPrint n_print)
        {
            if (n_print.isLocal)
                localPlayer = n_print;

            players.Add(n_print);
            n_print.transform.parent = multiplayerReferences;
        }

        #endregion Starting the Match

        #region Turn Management

        public void PlayerEndsPhase(int photonId)
        {
            photonView.RPC("RPC_PlayerEndsPhase", PhotonTargets.MasterClient, photonId);
        }

        [PunRPC]
        public void RPC_PlayerEndsPhase(int photonId)
        {
            // Master manage phases here or ends the turn for this player
            phaseIndex++;
            if ( phaseIndex > turnsHolder.phaseOrder.Length -1)
            {
                phaseIndex = 0;

                playerIndex++;
                if ( playerIndex > players.Count - 1)
                {
                    playerIndex = 0;
                }
            }
            photonView.RPC("RPC_SetPhaseToPlayer", PhotonTargets.All, players[playerIndex].photonID, turnsHolder.phaseOrder[phaseIndex]);

        }

        [PunRPC]
        public void RPC_SetPhaseToPlayer(int photonId, string phase)
        {
            if ( photonId == localPlayer.photonID )
            {
                Phase targetPhase = turnsHolder.GetPhase(phase);
                gm.SetCurrentPhase(targetPhase);
                targetPhase.OnStartPhase();
            }
            else
            {
                gm.SetCurrentPhase(null);
            }
        }

        public void PlayerEndsTurn(int photonId)
        {
            photonView.RPC("RPC_PlayerEndsTurn", PhotonTargets.MasterClient, photonId);
        }

        [PunRPC]
        public void RPC_PlayerEndsTurn(int photonId)
        {
            //if (photonId == gm.currentPlayer.photonId)
            //{
            //    if (NetworkManager.isMaster)
            //    {
            //        int targetId = gm.GetNextPlayerId();
            //        photonView.RPC("RPC_PlayerStartsTurn", PhotonTargets.All, targetId);
            //    }
            //}
        }

        [PunRPC]
        public void RPC_PlayerStartsTurn(int photonId)
        {
            //gm.ChangeCurrentTurn(photonId);
        }

        //@ duplicate? RPC_PlayerEndsPhase
        [PunRPC]
        public void RPC_PlayerEndPhase(int photonId)
        {
            NetworkPrint print = GetPlayer(photonId);
            if ( print.playerHolder == gm.currentPlayer )
            {
                gm.EndCurrentPhase();
            }
        }

        #endregion Turn Management

        #region Card Checks

        public void PlayerPicksCardFromDeck(PlayerHolder playerHolder)
        {
            NetworkPrint p = GetPlayer(playerHolder.photonId);

            Card c = p.deckCards[0];
            p.deckCards.RemoveAt(0);

            PlayerWantsToUseCard(c.instID, p.photonID, CardOpertation.pickCardFromDeck);
        }

        public void PlayerWantsToUseCard(int cardInst, int photonId, CardOpertation opertation)
        {
            photonView.RPC("RPC_PlayerWantsToUseCard", PhotonTargets.MasterClient, cardInst, photonId, opertation);
        }

        [PunRPC]
        public void RPC_PlayerWantsToUseCard(int cardInst, int photonId, CardOpertation opertation)
        {
            if (!NetworkManager.isMaster) return;

            bool hasCard = PlayerHasCard(cardInst, photonId);

            if ( hasCard )
            {
                photonView.RPC("RPC_PlayerUsesCard", PhotonTargets.All, cardInst, photonId, opertation);
            }

        }

        bool PlayerHasCard(int cardInst, int photonId)
        {
            NetworkPrint player = GetPlayer(photonId);
            Card c = player.GetCard(cardInst);
            return (c != null);
        }

        #endregion Card Checks

        #region Card Operations

        public enum CardOpertation
        {
            dropResourcesCard, pickCardFromDeck, dropCreatureCard, setCardForBattle, cardToGraveyard
        }

        [PunRPC]
        public void RPC_PlayerUsesCard(int instId, int photonId, CardOpertation operation)
        {
            NetworkPrint p = GetPlayer(photonId);
            Card card = p.GetCard(instId);

            switch (operation)
            {
                case CardOpertation.dropResourcesCard:
                    Settings.SetParentForCard(card.cardPhysicalInst.transform, p.playerHolder.currentHolder.resourcesGrid.value);
                    card.cardPhysicalInst.currentLogic = dataHolder.cardDownLogic;
                    p.playerHolder.AddResourceCard(card.cardPhysicalInst.gameObject);
                    card.cardPhysicalInst.gameObject.SetActive(true);
                    break;

                case CardOpertation.pickCardFromDeck:
                    GameObject go = Instantiate(dataHolder.cardPrefab) as GameObject;
                    CardViz v = go.GetComponent<CardViz>();
                    v.LoadCard(card);
                    card.cardPhysicalInst = go.GetComponent<CardInstance>();
                    card.cardPhysicalInst.currentLogic = dataHolder.handCard;
                    card.cardPhysicalInst.owner = p.playerHolder;

                    Settings.SetParentForCard(go.transform, p.playerHolder.currentHolder.handGrid.value);
                    p.playerHolder.handCards.Add(card.cardPhysicalInst);
                    break;

                case CardOpertation.dropCreatureCard:
                    bool canUse = p.playerHolder.CanUseCard(card);
                    if (canUse)
                    {
                        Settings.DropHeroCard(card.cardPhysicalInst.transform, p.playerHolder.currentHolder.downGrid.value, card.cardPhysicalInst);
                        card.cardPhysicalInst.currentLogic = dataHolder.cardDownLogic;
                    }
                    else
                    {
                        Settings.RegisterEvent("Not enough resources to use card", Color.red);
                    }
                    card.cardPhysicalInst.gameObject.SetActive(true);
                    break;

                case CardOpertation.setCardForBattle:
                    if (p.playerHolder.attackingCards.Contains(card.cardPhysicalInst))
                    {
                        p.playerHolder.attackingCards.Remove(card.cardPhysicalInst);
                        p.playerHolder.currentHolder.SetCardsOffBatlleLine(card.cardPhysicalInst);
                    }
                    else
                    {
                        if (card.cardPhysicalInst.CanAttack())
                        {
                            p.playerHolder.attackingCards.Add(card.cardPhysicalInst);
                            p.playerHolder.currentHolder.SetCardsOnBatlleLine(card.cardPhysicalInst);
                        }
                    }
                    break;

                case CardOpertation.cardToGraveyard:
                    card.cardPhysicalInst.CardInstanceToGraveyard();
                    break;

                default:
                    break;
            }
        }

        #endregion Card Operations

        #region Battle Resolve

        public void SetBattleResolvePhase()
        {
            photonView.RPC("RPC_BattleResolve", PhotonTargets.MasterClient);
        }

        [PunRPC]
        public void RPC_BattleResolve()
        {
            if (!NetworkManager.isMaster) return;

            BattleResolveForPlayers();
        }

        void BattleResolveForPlayers()
        {
            PlayerHolder player = Settings.gameManager.currentPlayer;
            PlayerHolder enemy = Settings.gameManager.GetEnemyOf(player);

            if (enemy.attackingCards.Count == 0)
            {
                photonView.RPC("RPC_BattleResolveCallback", PhotonTargets.All, enemy.photonId);
               // photonView.RPC("RPC_PlayerEndsPhase", PhotonTargets.All, player.photonId);

                return;
            }

            Dictionary<CardInstance, BlockInstance> blockDict = Settings.gameManager.GetBlockInstances();

            for (int i = 0; i < enemy.attackingCards.Count; i++)
            {
                CardInstance inst = enemy.attackingCards[i];
                Card c = inst.viz.card;
                CardProperties attack = c.GetProperty(dataHolder.attackElement);
                if (attack == null)
                {
                    Debug.LogError("You are attacking with a card that can't attack");
                    continue;
                }

                int damageValue = attack.intValue;

                BlockInstance bi = GetBlockInstanceOfAttacker(inst, gm.GetBlockInstances());
                if (bi != null)
                {
                    for (int b = 0; b < bi.blocker.Count; b++)
                    {
                        CardProperties def = c.GetProperty(gm.defenceProperty);
                        if (def == null)
                        {
                            Debug.LogWarning("You are trying to block with a card with no defense element!");
                            continue;
                        }

                        damageValue -= def.intValue;

                        if (def.intValue <= damageValue)
                        {
                            bi.blocker[b].CardInstanceToGraveyard();
                        }
                    }
                }

                if (damageValue <= 0)
                {
                    damageValue = 0;
                    PlayerWantsToUseCard(inst.viz.card.instID, enemy.photonId, CardOpertation.cardToGraveyard);
                }

                enemy.DropCard(inst, false);
                player.DoDamage(damageValue);
                photonView.RPC("RPC_SyncPlayerHealth", PhotonTargets.All, player.photonId, player.health);
            }

            photonView.RPC("RPC_BattleResolveCallback", PhotonTargets.All, enemy.photonId);
            //photonView.RPC("RPC_PlayerEndsPhase", PhotonTargets.All, player.photonId);

            return;
        }

        BlockInstance GetBlockInstanceOfAttacker(CardInstance attacker, Dictionary<CardInstance, BlockInstance> blockInstances)
        {
            BlockInstance r = null;
            blockInstances.TryGetValue(attacker, out r);

            return r;
        }

        [PunRPC]
        public void RPC_SyncPlayerHealth(int photonId, int health)
        {
            NetworkPrint p = GetPlayer(photonId);
            p.playerHolder.health = health;
            p.playerHolder.statsUI.UpdateHealth();
        }

        [PunRPC]
        public void RPC_BattleResolveCallback(int photonId)
        {
            foreach (NetworkPrint p in players)
            {
                //bool isAttacker = false;

                foreach (CardInstance c in p.playerHolder.attackingCards)
                {
                    p.playerHolder.currentHolder.SetCardsOffBatlleLine(c);
                    c.SetFlatfooted(true);
                }

                if (p.photonID == photonId)
                {
                    if (p == localPlayer)
                    {
                        // isAttacker = true;
                       // Settings.gameManager.EndCurrentPhase();
                    }
                }

                p.playerHolder.attackingCards.Clear();

            }

            foreach (BlockInstance bi in Settings.gameManager.GetBlockInstances().Values)
            {
                foreach (CardInstance c in bi.blocker)
                {
                    c.owner.currentHolder.SetCardsOffBatlleLine(c);
                }
            }

            Settings.gameManager.ClearBlockInstances();
            //p.attackingCards.Clear();
        }

        #endregion Battle Resolve

        #region Blocking

        public void PlayerBlocksTargetCard(int cardInst, int photonId, int targetInst, int blocked)
        {
            photonView.RPC("RPC_PlayerBlocksTargetCard_Master", PhotonTargets.MasterClient, cardInst, photonId, targetInst, blocked);
        }

        [PunRPC]
        public void RPC_PlayerBlocksTargetCard_Master(int cardInst, int photonId, int targetInst, int blocked)
        {
            NetworkPrint playerBlocker = GetPlayer(photonId);
            Card blockerCard = playerBlocker.GetCard(cardInst);

            NetworkPrint blockedPlayer = GetPlayer(blocked);
            Card blockedCard = blockedPlayer.GetCard(targetInst);

            int count = 0;
            Settings.gameManager.AddBlockInstance(blockedCard.cardPhysicalInst, blockerCard.cardPhysicalInst, ref count);

            photonView.RPC("RPC_PlayerBlocksTargetCard_Client", PhotonTargets.All, cardInst, photonId, targetInst, blocked, count);
        }

        [PunRPC]
        public void RPC_PlayerBlocksTargetCard_Client(int cardInst, int photonId, int targetInst, int blocked, int count)
        {
            NetworkPrint playerBlocker = GetPlayer(photonId);
            Card blockerCard = playerBlocker.GetCard(cardInst);

            NetworkPrint blockedPlayer = GetPlayer(blocked);
            Card blockedCard = playerBlocker.GetCard(targetInst);

            Settings.SetCardForBlock(blockerCard.cardPhysicalInst.transform, blockedCard.cardPhysicalInst.transform, count);
        }

        #endregion Blocking

        #region Multiple Card Operations

        #region Flatfooted Cards

        public void PlayerWantsToResetFlatfootedCards(int photonId)
        {
            photonView.RPC("RPC_ResetFlatfootedCardsForPlayer_Master", PhotonTargets.MasterClient, photonId);
        }

        [PunRPC]
        public void RPC_ResetFlatfootedCardsForPlayer_Master(int photonId)
        {
            NetworkPrint p = GetPlayer(photonId);
            if (gm.localPlayer == p.playerHolder)
            {
                photonView.RPC("RPC_ResetFlatfootedCardsForPlayer", PhotonTargets.All, photonId);
            }
        }

        [PunRPC]
        public void RPC_ResetFlatfootedCardsForPlayer(int photonId)
        {
            NetworkPrint p = GetPlayer(photonId);

            foreach (CardInstance c in p.playerHolder.downCards)
            {
                if (c.isFlatfooted)
                {
                    c.SetFlatfooted(false);
                }
            }
        }

        #endregion Flatfooted Cards

        #region Resources Cards

        public void PlayerWantsToResetResourcesCards(int photonId)
        {
            photonView.RPC("RPC_PlayerWantsToResetResourcesCards_Master", PhotonTargets.MasterClient, photonId);
        }

        [PunRPC]
        public void RPC_PlayerWantsToResetResourcesCards_Master(int photonId)
        {
            NetworkPrint p = GetPlayer(photonId);
            if (gm.localPlayer == p.playerHolder)
            {
                photonView.RPC("RPC_ResetResourcesCardsForPlayer", PhotonTargets.All, photonId);
            }
        }

        [PunRPC]
        public void RPC_ResetResourcesCardsForPlayer(int photonId)
        {
            NetworkPrint p = GetPlayer(photonId);

            p.playerHolder.MakeAllResourceCardUsable();
        }

        #endregion Resources Cards

        #region Management

        public void SendPhase(string phase, string holder)
        {
            //photonView.RPC("RPC_MessagePhase", PhotonTargets.All, phase, holder);
        }

        [PunRPC]
        public void RPC_MessagePhase(string phase, string holder)
        {
            //Debug.Log(phase + " " + holder);
        }

        #endregion Management

        #endregion Multiple Card Operations

    }

}