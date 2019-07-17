using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Legendary
{

    public class NetworkPrint : Photon.MonoBehaviour
    {
        public int photonID;
        public bool isLocal;

        string[] cardIDs;
        public string[] GetStartingCardsIDs()
        {
            return cardIDs;
        }

        public PlayerHolder playerHolder;
        Dictionary<int, Card> myCards = new Dictionary<int, Card>();
        public List<Card> deckCards = new List<Card>();

        public void AddCard(Card c)
        {
            myCards.Add(c.instID, c);
            deckCards.Add(c);
        }

        public Card GetCard(int instId)
        {
            Card c = null;
            myCards.TryGetValue(instId, out c);

            return c;
        }

        void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            photonID = photonView.ownerId;
            isLocal = photonView.isMine;

            object[] data = photonView.instantiationData;
            cardIDs = (string[]) data[0];

            MultiplayerManager.singleton.AddPlayer(this);

        }

    }

}