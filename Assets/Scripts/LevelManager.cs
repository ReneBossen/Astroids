using Mirror;
using System;

namespace Assets.Scripts
{
    public class LevelManager : NetworkBehaviour
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
        }

        public override void OnStartServer()
        {
            GameManager.Instance.OnStartGame += GameManager_OnStartGame;
            GameManager.Instance.OnRestartGame += GameManager_OnRestartGame;

            _astroidsToSpawn = 2;
        }

        [Server]
        private void GameManager_OnStartGame(object sender, EventArgs e)
        {
            StartGame();
        }

        [Server]
        private void GameManager_OnRestartGame(object sender, EventArgs e)
        {
            StartGame();
        }

        [Server]
        private void StartGame()
        {
            CurrentLevel = 1;
            StartLevel();
        }

        [Server]
        private void StartLevel()
        {
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

        [Server]
        private void NextLevel()
        {
            CurrentLevel++;
            StartLevel();
        }
    }
}