using Assets.Scripts.Network;
using Assets.Scripts.UI;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class GameManager : NetworkBehaviour
    {
        public static GameManager Instance { get; private set; }

        public event EventHandler OnStartGame;
        public event EventHandler OnRestartGame;
        public event EventHandler OnGameOver;
        public event EventHandler OnShowGameOverUI;

        [SyncVar]
        public bool GameIsRunning = false;

        [SerializeField] private GameObject _playerPrefab;

        private List<GameObject> _players = new();
        public List<GameObject> Players => _players;

        private AstroidsNetworkManager _networkManager;


        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            Instance = this;
        }

        public override void OnStartServer()
        {
            _networkManager = FindAnyObjectByType<AstroidsNetworkManager>();

            Health.Instance.OnPlayerDeath += Health_OnPlayerDeath;
            GameOverUIManager.Instance.OnRestartGame += GameOverUIManager_OnRestartGame;
        }

        [ClientRpc]
        private void InitializeClientUISubscribers()
        {
            UIManager.Instance.InitializeUISubscribers();
        }

        private void GameOverUIManager_OnRestartGame(object sender, EventArgs e)
        {
            GameIsRunning = true;
            RestartGameRpc();
        }

        private void Health_OnPlayerDeath(object sender, EventArgs e)
        {
            GameOver();
        }

        [Server]
        public void ServerStartGame()
        {
            if (GameIsRunning)
                return;

            Debug.Log("[GM] Game Started!");
            _networkManager.SpawnWaitingPlayers();
            InitializeClientUISubscribers();
            StartGameRpc();
        }

        [ClientRpc]
        private void StartGameRpc()
        {
            OnStartGame?.Invoke(this, EventArgs.Empty);
        }

        [ClientRpc]
        private void RestartGameRpc()
        {
            OnRestartGame?.Invoke(this, EventArgs.Empty);
        }

        private void GameOver()
        {
            OnGameOverRpc();
            GameIsRunning = false;
        }

        [ClientRpc]
        private void OnGameOverRpc()
        {
            OnGameOver?.Invoke(this, EventArgs.Empty);
            StartCoroutine(OnShowGameOverUIRpc());
        }

        [Client]
        private IEnumerator OnShowGameOverUIRpc()
        {
            yield return new WaitForSeconds(1);
            OnShowGameOverUI?.Invoke(this, EventArgs.Empty);
        }

        public class ApplicationQuitHandler : MonoBehaviour
        {
            void OnApplicationQuit()
            {
                if (!Application.isEditor)
                {
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                }
            }
        }
    }
}