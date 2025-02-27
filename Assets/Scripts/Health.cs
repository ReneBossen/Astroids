using Assets.Scripts.UI;
using Mirror;
using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class Health : NetworkBehaviour
    {
        public static Health Instance { get; private set; }

        public event EventHandler OnPlayerDeath;
        public event EventHandler OnPlayerTakeDamage;
        public event EventHandler OnRetartGame;

        [SyncVar]
        private int _currentHealth;
        public int CurrentHealth => _currentHealth;

        private int _maxHealth;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            Instance = this;

            _maxHealth = 3;
            _currentHealth = _maxHealth;
        }

        private void Start()
        {
            AstroidManager.Instance.OnPlayerHitByAstroid += AstroidManager_OnPlayerHitByAstroid;
            GameOverUIManager.Instance.OnRestartGame += GameOverUIManager_OnRestartGame;
        }

        private void GameOverUIManager_OnRestartGame(object sender, EventArgs e)
        {
            RestartGame();
            OnRetartGame?.Invoke(this, EventArgs.Empty);
        }

        private void AstroidManager_OnPlayerHitByAstroid(object sender, EventArgs e)
        {
            TakeDamage();
        }

        private void RestartGame()
        {
            _currentHealth = _maxHealth;
        }

        private void TakeDamage()
        {
            _currentHealth--;

            OnPlayerTakeDamage?.Invoke(this, EventArgs.Empty);

            CheckIfDead();
        }

        private void CheckIfDead()
        {
            if (_currentHealth > 0)
                return;

            Debug.Log("Player dead");
            OnPlayerDeath?.Invoke(this, EventArgs.Empty);
        }
    }
}