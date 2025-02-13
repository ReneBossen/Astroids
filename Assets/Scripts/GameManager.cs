using Assets.Scripts.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public event EventHandler OnStartGame;
        public event EventHandler OnRestartGame;
        public event EventHandler OnGameOver;

        public bool GameIsRunning { get; private set; } = false;

        [SerializeField] private GameObject _playerPrefab;

        private GameObject _player;


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
            StartGameUIManager.Instance.OnStartGame += StartGameUIManager_OnStartGame;
            GameOverUIManager.Instance.OnRestartGame += GameOverUIManager_OnRestartGame;
            Health.Instance.OnPlayerDeath += Health_OnPlayerDeath;
        }

        private void StartGameUIManager_OnStartGame(object sender, EventArgs e)
        {
            StartGame();
        }

        private void GameOverUIManager_OnRestartGame(object sender, EventArgs e)
        {
            RestartGame();
        }

        private void Health_OnPlayerDeath(object sender, EventArgs e)
        {
            GameOver();
        }

        private void StartGame()
        {
            SpawnPlayer();
            OnStartGame?.Invoke(this, EventArgs.Empty);
            GameIsRunning = true;
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

        private void SpawnPlayer()
        {
            GameObject player = Instantiate(_playerPrefab, Vector3.zero, Quaternion.identity);
            _player = player;
        }

        public GameObject GetActivePlayer()
        {
            return _player;
        }
    }
}