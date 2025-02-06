using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class Health : MonoBehaviour
    {
        public static Health Instance { get; private set; }

        public event EventHandler OnPlayerDeath;
        public event EventHandler OnPlayerTakeDamage;
        public int CurrentHealth { get; private set; }

        private int _maxHealth;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            Instance = this;

            _maxHealth = 3;
            CurrentHealth = _maxHealth;
        }

        private void Start()
        {
            AstroidManager.Instance.OnPlayerHitByAstroid += AstroidManager_OnPlayerHitByAstroid;
        }

        private void AstroidManager_OnPlayerHitByAstroid(object sender, EventArgs e)
        {
            TakeDamage();
        }

        private void TakeDamage()
        {
            CurrentHealth--;

            OnPlayerTakeDamage?.Invoke(this, EventArgs.Empty);

            CheckIfDead();
        }

        private void CheckIfDead()
        {
            if (CurrentHealth > 0)
                return;

            Debug.Log("Player dead");
            OnPlayerDeath?.Invoke(this, EventArgs.Empty);
        }
    }
}