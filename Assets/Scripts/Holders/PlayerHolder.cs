using System.Collections.Generic;
using UnityEngine;

namespace Legendary
{

    [CreateAssetMenu(menuName = "Holders/Player Holder")]
    public class PlayerHolder : ScriptableObject
    {
        public string userName;
        public Sprite portrait;
        public Color playerColor;
        [System.NonSerialized] public int health = 20;

        public PlayerStatsUI statsUI;

        // public string[] startingCards;
        public List<string> startingDeck = new List<string>();
        [System.NonSerialized] public List<string> all_Cards = new List<string>();

        public int resourcesPerTurn = 1;
        [System.NonSerialized] public int resourcesDroppedThisTurn;

        public bool isHumanPlayer;

        public GameElements.GE_Logic handLogic;
        public GameElements.GE_Logic downLogic;

        [System.NonSerialized] public CardHolders currentHolder;

        [System.NonSerialized] public List<CardInstance> handCards = new List<CardInstance>();
        [System.NonSerialized] public List<CardInstance> downCards = new List<CardInstance>();
        [System.NonSerialized] public List<CardInstance> attackingCards = new List<CardInstance>();
        [System.NonSerialized] public List<ResourceHolder> resourcesList = new List<ResourceHolder>();

        public int resourcesCount
        {
            get { return currentHolder.resourcesGrid.value.GetComponentsInChildren<CardViz>().Length; }
        }

        public void Init()
        {
            health = 20;
            all_Cards.AddRange(startingDeck);
        }


        public void AddResourceCard(GameObject cardObj)
        {
            ResourceHolder resourceHolder = new ResourceHolder
            {
                cardObj = cardObj
            };

            resourcesList.Add(resourceHolder);
            resourcesDroppedThisTurn++;

            Settings.RegisterEvent(userName + " drops resources card", playerColor);

        }

        public int NonUsedCards()
        {
            int result = 0;

            for (int i = 0; i < resourcesList.Count; i++)
            {
                if ( !resourcesList[i].isUsed )
                {
                    result++;
                }
            }

            return result;
        }

        public bool CanUseCard(Card c)
        {
            bool result = false;

            if (c.cardType is HeroCard)
            {
                int currentResources = NonUsedCards();
                if (c.cost <= currentResources)
                    result = true;
            }
            else
            {
                if (c.cardType is BystanderCard)
                {
                    if (resourcesPerTurn - resourcesDroppedThisTurn > 0 )
                    {
                        result = true;
                    }
                }
            }
            return result;
        }

        public void DropCard(CardInstance inst, bool registerEvent = true)
        {
            if (handCards.Contains(inst))
                handCards.Remove(inst);

            downCards.Add(inst);

            if ( registerEvent )
                Settings.RegisterEvent(userName + " used " + inst.viz.card.name + " for " + inst.viz.card.cost + " resources", playerColor);

        }

        public List<ResourceHolder> GetUnusedResources()
        {
            List<ResourceHolder> result = new List<ResourceHolder>();

            for (int i = 0; i < resourcesList.Count; i++)
            {
                if ( !resourcesList[i].isUsed)
                {
                    result.Add(resourcesList[i]);
                }
            }
            return result;
        }

        public void MakeAllResourceCardUsable()
        {
            for (int i = 0; i < resourcesList.Count; i++)
            {
                resourcesList[i].isUsed = false;
                resourcesList[i].cardObj.transform.localEulerAngles = Vector3.zero;
            }
            resourcesDroppedThisTurn = 0;
        }

        public void UseResourceCards(int amount)
        {
            Vector3 euler = new Vector3(0, 0, 90);

            List<ResourceHolder> l = GetUnusedResources();
            for (int i = 0; i < amount; i++)
            {
                l[i].isUsed = true;
                l[i].cardObj.transform.localEulerAngles = euler;
            }
        }

        public void LoadPlayerOnStatsUI()
        {
            if (statsUI != null)
            {
                statsUI.player = this;
                statsUI.UpdateAll();
            }
        }

        public void DoDamage(int v)
        {
            health -= v;

            if (statsUI != null)
                statsUI.UpdateHealth();
        }

       
    }

}
