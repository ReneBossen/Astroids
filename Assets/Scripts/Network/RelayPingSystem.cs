using Assets.Scripts.GameCriticals;
using System;
using System.Collections;
using Unity.Networking.Transport;
using UnityEngine;
using NetworkConnection = Unity.Networking.Transport.NetworkConnection;

namespace Assets.Scripts.Network
{
    public class RelayPingSystem : MonoBehaviour
    {
        public NetworkDriver driver;
        public NetworkConnection connection;
        private bool isRelayServerConnected = false;

        private void Start()
        {
            StartCoroutine(WaitForGameManager());
        }

        private IEnumerator WaitForGameManager()
        {
            yield return new WaitWhile(() => GameManager.Instance == null);

            GameManager.Instance.OnStartGame += GameManager_OnStartGame;
        }

        private void GameManager_OnStartGame(object sender, EventArgs e)
        {
            driver = NetworkDriver.Create();
            connection = default;

            isRelayServerConnected = true;
        }

        private void Update()
        {
            if (!isRelayServerConnected)
                return;

            driver.ScheduleUpdate().Complete();
        }
    }
}