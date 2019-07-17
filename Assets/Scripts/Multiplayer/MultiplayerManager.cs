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
                    StartMatch();
                }
            }
        }

        #endregion Tick

        #region Init
        void OnPhotonInstantiate(PhotonMessageInfo info)
        {
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

        #region Starting the match

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
            gm.InitGame(startingPlayer);
        }

        public void AddPlayer(NetworkPrint n_print)
        {
            if (n_print.isLocal)
                localPlayer = n_print;

            players.Add(n_print);
            n_print.transform.parent = multiplayerReferences;
        }

        #endregion Starting the match

        #region End Turn

        public void PlayerEndsTurn(int photonId)
        {
            photonView.RPC("RPC_PlayerEndsTurn", PhotonTargets.MasterClient, photonId);
        }

        [PunRPC]
        public void RPC_PlayerEndsTurn(int photonId)
        {
            if (photonId == gm.currentPlayer.photonId)
            {
                if (NetworkManager.isMaster)
                {
                    int targetId = gm.GetNextPlayerId();
                    photonView.RPC("RPC_PlayerStartsTurn", PhotonTargets.All, targetId);
                }
            }
        }

        [PunRPC]
        public void RPC_PlayerStartsTurn(int photonId)
        {
            gm.ChangeCurrentTurn(photonId);
        }

        #endregion End Turn

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
            dropResourcesCard, pickCardFromDeck
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
                    Settings.SetParentForCard(go.transform, p.playerHolder.currentHolder.handGrid.value);
                    p.playerHolder.handCards.Add(card.cardPhysicalInst);
                    break;

                default:
                    break;
            }

        }

        #endregion Card Operations
    }

}