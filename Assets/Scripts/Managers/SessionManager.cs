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

        public void LoadGameLevel()
        {
            StartCoroutine("scene1");
        }

        public void LoadMenu()
        {
            StartCoroutine("menu");
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