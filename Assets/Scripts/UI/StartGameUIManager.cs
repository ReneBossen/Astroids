using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class StartGameUIManager : UIManagerBaseClass
    {
        public static StartGameUIManager Instance { get; private set; }

        public event EventHandler OnStartGame;

        [SerializeField] private Button _readyButton;

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
            _readyButton.onClick.AddListener(() =>
            {
                OnStartGame?.Invoke(this, EventArgs.Empty);
            });
        }
    }
}