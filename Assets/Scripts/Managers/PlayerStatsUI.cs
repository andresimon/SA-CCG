using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Legendary
{

    public class PlayerStatsUI : MonoBehaviour
    {

        public PlayerHolder player;
        public Image playerPortrait;
        public Text health;
        public Text userName;

        public void UpdateAll()
        {
            UpdateUserName();
            UpdateHealth();
        }

        public void UpdateUserName()
        {
            userName.text = player.userName;
            playerPortrait.sprite = player.portrait;
        }

        public void UpdateHealth()
        {
            health.text = player.health.ToString();
        }
    }

}