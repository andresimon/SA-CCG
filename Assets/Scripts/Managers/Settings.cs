using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Legendary
{
    public static class Settings
    {
        public static GameManager gameManager;

        private static PropertiesManager _propertiesManager;

        public static PropertiesManager GetPropertiesManager()
        {
            if (_propertiesManager == null )
            {
                _propertiesManager = Resources.Load("PropertiesManager") as PropertiesManager;
            }

            return _propertiesManager;
        }

        public static List<RaycastResult> GetUIObjs()
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            return results;
        }
    }
}