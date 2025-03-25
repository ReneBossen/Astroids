using Assets.Scripts.Network;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        public static MainMenuUI Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            AstroidsNetworkManager.OnDisconnect += AstroidsNetworkManager_OnDisconnect;
        }

        private void AstroidsNetworkManager_OnDisconnect(object sender, EventArgs e)
        {
            NetworkUI.Instance.InitializeNetworkUI();
        }

        public void InitializeUISubscribers()
        {
            NetworkUI.Instance.InitializeSubscribers();
        }
    }
}