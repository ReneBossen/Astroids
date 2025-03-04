using Assets.Scripts.UI;
using Mirror;
using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class Score : NetworkBehaviour
    {
        public static Score Instance { get; private set; }

        public event Action<int> OnScoreUpdated;

        [SyncVar(hook = nameof(UpdateScoreUI))]
        private int _currentScore;
        public int CurrentScore => _currentScore;

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
            _currentScore = 0;
            AstroidManager.Instance.OnAstroidDestroyed += AstroidManager_OnAstroidDestroyed;
            GameManager.Instance.OnRestartGame += GameManager_OnRestartGame;
        }

        [Server]
        private void GameManager_OnRestartGame(object sender, System.EventArgs e)
        {
            ResetScore();
        }

        [Server]
        private void AstroidManager_OnAstroidDestroyed(object sender, AstroidManager.OnAstroidDestroyedEventArgs e)
        {
            AddScore(e.score);
        }

        [Server]
        private void AddScore(int amount)
        {
            _currentScore += amount;
        }

        [Server]
        private void ResetScore()
        {
            _currentScore = 0;
        }

        private void UpdateScoreUI(int oldScore, int newScore)
        {
            OnScoreUpdated?.Invoke(CurrentScore);
        }
    }
}