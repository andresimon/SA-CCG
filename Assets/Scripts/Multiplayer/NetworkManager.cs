using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Legendary
{

    public class NetworkManager : MonoBehaviour
    {
        public bool isMaster;
        public static NetworkManager singleton;

        List<MultiplayerHolder> multiplayerHolders = new List<MultiplayerHolder>();

        ResourcesManager rm;
        int CardInstIDs;

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

            void CreateCardClient_call(string cardID, int instID, int photonID)
            {
                Card c = CreateCardCliente(cardID, instID);
                if ( c != null )
                {
                    MultiplayerHolder h = GetHolder(photonID);
                    h.RegisterCard(c);
                }
            }

            Card CreateCardCliente(string cardID, int instID)
            {
                Card card = rm.GetCardInstance(cardID);
                card.instID = instID;

                return card;
            }

        #endregion My Calls

        #region Photon Callbacks
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