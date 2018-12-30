using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Legendary
{
    public static class Settings
    {
        private static PropertiesManager _propertiesManager;

        public static PropertiesManager GetPropertiesManager()
        {
            if (_propertiesManager == null )
            {
                _propertiesManager = Resources.Load("PropertiesManager") as PropertiesManager;
            }

            return _propertiesManager;
        }
    }
}