using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Legendary
{

    public class ConsoleManager : MonoBehaviour
    {
        public Transform consoleGrid;
        public GameObject prefab;
        Text[] txtObjcts;
        int index;

        public ConsoleHook hook;

        private void Awake()
        {
            hook.consoleManager = this;

            txtObjcts = new Text[5];

            for (int i = 0; i < txtObjcts.Length; i++)
            {
                GameObject go = Instantiate(prefab) as GameObject;
                txtObjcts[i] = go.GetComponent<Text>();
                go.transform.SetParent(consoleGrid.transform);
            }
        }

        public void RegisterEvent(string s, Color color)
        {
            index++;
            if (index > txtObjcts.Length - 1)
                index = 0;

            txtObjcts[index].color = color;
            txtObjcts[index].text = s;
            txtObjcts[index].gameObject.SetActive(true);
            txtObjcts[index].transform.SetAsLastSibling();
        }
    }

}
