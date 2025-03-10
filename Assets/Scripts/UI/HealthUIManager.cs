using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class HealthUIManager : UIManagerBaseClass
    {
        public static HealthUIManager Instance { get; private set; }

        [SerializeField] private List<Image> _healthImages;

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
            Health.Instance.OnPlayerTakeDamage += Health_OnPlayerTakeDamage;
            Health.Instance.OnRetartGame += Health_OnRetartGame;
        }

        private void Health_OnRetartGame(object sender, EventArgs e)
        {
            UpdateHealthUI();
        }

        private void Health_OnPlayerTakeDamage(object sender, EventArgs e)
        {
            UpdateHealthUI();
        }

        private void UpdateHealthUI()
        {
            int currentHealth = Health.Instance.CurrentHealth;

            for (int i = 0; i < _healthImages.Count; i++)
            {
                _healthImages[i].enabled = i < currentHealth;
            }
        }
    }
}