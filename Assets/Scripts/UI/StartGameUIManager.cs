using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class StartGameUIManager : UIManagerBaseClass
    {
        public static StartGameUIManager Instance { get; private set; }

        public event EventHandler OnStartGame;

        [SerializeField] private Button _startButton;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            NetworkUI.Instance.OnHostGame += NetworkUI_OnHostGame;
            _startButton.onClick.AddListener(() =>
            {
                OnStartGame?.Invoke(this, EventArgs.Empty);

                _startButton.gameObject.SetActive(false);
            });

            _startButton.gameObject.SetActive(false);
        }

        private void NetworkUI_OnHostGame(object sender, EventArgs e)
        {
            _startButton.gameObject.SetActive(true);
        }
    }
}