using SO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Legendary
{

    public class NetworkManager : Photon.PunBehaviour
    {
        public static bool isMaster;
        public static NetworkManager singleton;

        List<MultiplayerHolder> multiplayerHolders = new List<MultiplayerHolder>();

        ResourcesManager rm;
        int CardInstIDs;

        public StringVariable logger;
        public GameEvent loggerUpdated;
        public GameEvent failedToConnect;
        public GameEvent onConnected;
        public GameEvent waitingForPlayer;

        private void Awake()
        {
            //rm = Settings.GetResourcesManager();
            if (singleton == null)
            {
                rm = Resources.Load("ResourcesManager") as ResourcesManager;
                singleton = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        private void Start()
        {
            rm.Init();
            PhotonNetwork.autoCleanUpPlayerObjects = false;
            PhotonNetwork.autoJoinLobby = false;
            PhotonNetwork.automaticallySyncScene = false;
            Init();
        }

        public void Init()
        {
            PhotonNetwork.ConnectUsingSettings("1.0");
            logger.value = "Connecting";
            loggerUpdated.Raise();
        }

        public MultiplayerHolder GetHolder(int photonID)
        {
            for (int i = 0; i < multiplayerHolders.Count; i++)
            {
                if (multiplayerHolders[i].onwerID == photonID)
                    return multiplayerHolders[i];
            }

            return null;
        }

        public Card GetCard(int instID, int ownerID)
        {
            MultiplayerHolder h = GetHolder(ownerID);
        //    return h.GetCard(instID);
            return (GetHolder(ownerID) as MultiplayerHolder).GetCard(instID);
        }

        #region My Calls

        public void OnPlayGame()
        {
            JoinRandomRoom();
        }

        void JoinRandomRoom()
        {
            PhotonNetwork.JoinRandomRoom();
        }

        void CreateRoom()
        {
            RoomOptions room = new RoomOptions();
            room.MaxPlayers = 2;

            PhotonNetwork.CreateRoom(RandomString(256), room, TypedLobby.Default);
        }

        private System.Random random = new System.Random();
        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWYZ0123456789abcedfgolkip";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // Master only
        public void PlayerJoined(int photonID, string[] cards)
        {
            MultiplayerHolder m = new MultiplayerHolder();
            m.onwerID = photonID;

            for (int i = 0; i < cards.Length; i++)
            {
                Card c = CreateCardsMaster(cards[i]);
                if (c == null) continue;

                m.RegisterCard(c);
                // RPC
            }
        }

        Card CreateCardsMaster(string cardID)
        {
            Card card = rm.GetCardInstance(cardID);
            card.instID = CardInstIDs;
            CardInstIDs++;

            return card;
        }

        //void CreateCardClient_call(string cardID, int instID, int photonID)
        //{
        //    Card c = CreateCardCliente(cardID, instID);
        //    if ( c != null )
        //    {
        //        MultiplayerHolder h = GetHolder(photonID);
        //        h.RegisterCard(c);
        //    }
        //}

        //Card CreateCardCliente(string cardID, int instID)
        //{
        //    Card card = rm.GetCardInstance(cardID);
        //    card.instID = instID;

        //    return card;
        //}

        #endregion My Calls

        #region Photon Callbacks

        public override void OnConnectedToMaster()
        {
            base.OnConnectedToMaster();
            logger.value = "Connected";
            loggerUpdated.Raise();
            onConnected.Raise();
        }

        public override void OnFailedToConnectToPhoton(DisconnectCause cause)
        {
            base.OnFailedToConnectToPhoton(cause);
            logger.value = "Failed To Connect";
            loggerUpdated.Raise();
            failedToConnect.Raise();
        }

        public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
        {
            base.OnPhotonRandomJoinFailed(codeAndMsg);
            CreateRoom();
        }

        public override void OnCreatedRoom()
        {
            base.OnCreatedRoom();
            isMaster = true;
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            logger.value = "Waiting for player";
            loggerUpdated.Raise();
            waitingForPlayer.Raise();
        }

        public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
        {
            if ( isMaster )
            {
                if ( PhotonNetwork.playerList.Length > 1)
                {
                    logger.value = "Ready for match";
                    loggerUpdated.Raise();

                    PhotonNetwork.room.IsOpen = false;
                    PhotonNetwork.Instantiate("MultiplayerManager", Vector3.zero, Quaternion.identity, 0);
                }
            }
        }

        public void LoadGameScene()
        {
            SessionManager.singleton.LoadGameLevel(OnGameSceneLoad);
        }

        void OnGameSceneLoad()
        {
            MultiplayerManager.singleton.countPlayers = true;
        }

        public override void OnDisconnectedFromPhoton()
        {
            base.OnDisconnectedFromPhoton();
        }

        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
        }

        #endregion Photon Callbacks

        #region RPCs

        #endregion RPCs

    }

    public class MultiplayerHolder
    {
        public int onwerID;
        Dictionary<int, Card> cards = new Dictionary<int, Card>();

        public void RegisterCard(Card c)
        {
            cards.Add(c.instID, c);
        }

        public Card GetCard(int instID)
        {
            Card r = null;
            cards.TryGetValue(instID, out r);

            return r;
        }
    }
}