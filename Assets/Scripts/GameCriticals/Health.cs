using Assets.Scripts.Astroids;
using Assets.Scripts.UI;
using Mirror;
using System;
using UnityEngine;

namespace Assets.Scripts.GameCriticals
{
    public class Health : NetworkBehaviour
    {
        public static Health Instance { get; private set; }

        public event EventHandler OnPlayerDeath;
        public event EventHandler OnPlayerTakeDamage;
        public event EventHandler OnRetartGame;

        [SyncVar(hook = nameof(OnCurrentHealthChanged))]
        private int _currentHealth;
        public int CurrentHealth => _currentHealth;

        private int _maxHealth;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                Debug.Log($"[HEALTH] Instance set");
            }
            else
            {
                Destroy(gameObject);
            }

            _maxHealth = 3;
            _currentHealth = _maxHealth;
        }

        public override void OnStartServer()
        {
            AstroidManager.Instance.OnPlayerHitByAstroid += AstroidManager_OnPlayerHitByAstroid;
            GameOverUIManager.Instance.OnRestartGame += GameOverUIManager_OnRestartGame;
        }

        [Server]
        private void GameOverUIManager_OnRestartGame(object sender, EventArgs e)
        {
            RestartGame();
            OnRetartGame?.Invoke(this, EventArgs.Empty);
        }

        [Server]
        private void AstroidManager_OnPlayerHitByAstroid(object sender, EventArgs e)
        {
            TakeDamage();
        }

        [Server]
        private void RestartGame()
        {
            _currentHealth = _maxHealth;
        }

        [Server]
        private void TakeDamage()
        {
            _currentHealth--;
            CheckIfDead();
        }

        private void OnCurrentHealthChanged(int oldValue, int newValue)
        {
            OnPlayerTakeDamage?.Invoke(this, EventArgs.Empty);
        }

        [Server]
        private void CheckIfDead()
        {
            if (_currentHealth > 0)
                return;

            Debug.Log("Player dead");
            OnPlayerDeath?.Invoke(this, EventArgs.Empty);
        }
    }
}