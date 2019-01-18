using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Legendary
{

    public class CardViz : MonoBehaviour
    {
        public Card card;
        public CardVizProperties[] properties;
        public GameObject[] HeroPropertiesHolder;

        public void LoadCard(Card c)
        {
            if (c == null) return;

            c.cardViz = this;

            card = c;

            DisableAll();

         //   c.cardType.OnSetType(this);

            for (int i = 0; i < c.properties.Length; i++)
            {
                CardProperties cp = c.properties[i];
                CardVizProperties p = GetProperty(cp.element);
             
                if (p == null) continue;
                
                if (cp.element is ElementInt)
                {
                    p.text.text = cp.intValue.ToString();
                    p.text.gameObject.SetActive(true);
                }
                else
                if (cp.element is ElementText)
                {
                    p.text.text = cp.stringValue;
                    p.text.gameObject.SetActive(true);
                }
                else
                if (cp.element is ElementImage)
                {
                    p.img.sprite = cp.sprite;
                    p.img.gameObject.SetActive(true);
                }

            }

        }

        public CardVizProperties GetProperty(Element e)
        {
            CardVizProperties result = null;

            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i].element == e)
                {
                    result = properties[i];
                    break;
                }
            }
            return result;
        }

        public void DisableAll()
        {
            foreach (CardVizProperties item in properties)
            {
                if (item.text != null)
                    item.text.gameObject.SetActive(false);

                if (item.img != null)
                    item.img.gameObject.SetActive(false);
            }
        }
    }
}
