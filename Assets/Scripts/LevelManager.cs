using Mirror;
using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance { get; private set; }

        public int CurrentLevel { get; private set; } = 0;

        private int _astroidsToSpawn;
        private int _astroidsRemaining;

        public event EventHandler<OnLevelStartedEventArgs> OnLevelStarted;
        public class OnLevelStartedEventArgs : EventArgs
        {
            public int Level;
            public int AstroidsRemaining;
        }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            Instance = this;

            _astroidsToSpawn = 2;
        }

        private void Start()
        {
            GameManager.Instance.OnStartGame += GameManager_OnStartGame;
            GameManager.Instance.OnRestartGame += GameManager_OnRestartGame;
        }

        private void GameManager_OnStartGame(object sender, EventArgs e)
        {
            StartGame();
        }

        private void GameManager_OnRestartGame(object sender, EventArgs e)
        {
            StartGame();
        }

        private void StartGame()
        {
            CurrentLevel = 1;
            StartLevel();
        }

        private void StartLevel()
        {
            Debug.Log($"[LVLMNG] StartLevel called");
            _astroidsRemaining = _astroidsToSpawn * CurrentLevel;
            OnLevelStarted?.Invoke(this, new OnLevelStartedEventArgs
            {
                Level = CurrentLevel,
                AstroidsRemaining = _astroidsRemaining
            });
        }

        [Server]
        public void AsteroidDestroyed()
        {
            _astroidsRemaining--;

            if (_astroidsRemaining <= 0)
            {
                NextLevel();
            }
        }

        private void NextLevel()
        {
            CurrentLevel++;
            StartLevel();
        }
    }
}