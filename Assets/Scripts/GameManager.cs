using Assets.Scripts.Network;
using Assets.Scripts.UI;
using Mirror;
using System;
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

        private void Start()
        {
            GameOverUIManager.Instance.OnRestartGame += GameOverUIManager_OnRestartGame;
            Health.Instance.OnPlayerDeath += Health_OnPlayerDeath;
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            _networkManager = FindAnyObjectByType<AstroidsNetworkManager>();
        }

        private void GameOverUIManager_OnRestartGame(object sender, EventArgs e)
        {
            RestartGame();
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
            StartGameRpc();
        }

        [ClientRpc]
        private void StartGameRpc()
        {
            OnStartGame?.Invoke(this, EventArgs.Empty);
        }

        private void RestartGame()
        {
            OnRestartGame?.Invoke(this, EventArgs.Empty);
            GameIsRunning = true;
        }

        private void GameOver()
        {
            OnGameOver?.Invoke(this, EventArgs.Empty);
            GameIsRunning = false;
        }
    }
}