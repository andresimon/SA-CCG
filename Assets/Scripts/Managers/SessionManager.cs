using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Legendary
{

    public class SessionManager : MonoBehaviour
    {
        public static SessionManager singleton;
        public delegate void OnSceneLoad();
        public OnSceneLoad onSceneLoad;

        private void Awake()
        {
            if (singleton == null)
            {
                singleton = this;

                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        public void LoadGameLevel(OnSceneLoad callback)
        {
            onSceneLoad = callback;
            StartCoroutine(LoadLevel("scene1"));
        }

        public void LoadMenu()
        {
            StartCoroutine(LoadLevel("menu"));
        }

        IEnumerator LoadLevel(string level)
        {
            yield return SceneManager.LoadSceneAsync(level, LoadSceneMode.Single);

            if ( onSceneLoad != null )
            {
                onSceneLoad();
                onSceneLoad = null;
            }
        }
    }

}