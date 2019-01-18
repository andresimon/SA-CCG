using UnityEngine;
using System.Collections;

namespace Legendary
{

    public class EventManager : MonoBehaviour
    {
        #region My Calls

            public void CardIsDroppedDown(int instID, int ownerID)
            {
                Card c = NetworkManager.singleton.GetCard(instID, ownerID);
            }

            public void CardIsPickedUpFromDeck(int instID, int ownerID)
            {
                Card c = NetworkManager.singleton.GetCard(instID, ownerID);
            }



        #endregion My Calls
    }

}