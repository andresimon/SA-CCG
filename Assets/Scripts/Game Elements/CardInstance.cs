using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Legendary
{
    public class CardInstance : MonoBehaviour, IClickable
    {
        public void OnClick()
        {
            throw new System.NotImplementedException();
        }

        public void OnHighligth()
        {
            Vector3 s = Vector3.one * 2;
            this.transform.localScale = s;
        }
    }
}