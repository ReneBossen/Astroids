using System;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [SerializeField] private GameOverUIManager _gameOverUIManager;
        [SerializeField] private HealthUIManager _healthUIManager;
        [SerializeField] private NetworkUI _networkUI;
        [SerializeField] private ScoreUIManager _scoreUIManager;

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
            HideGameUI();
            HideGameOverUI();
        }

        public void InitializeUISubscribers()
        {
            InitializeSubscribers();
            _gameOverUIManager.InitializeSubscribers();
            _healthUIManager.InitializeSubscribers();
            _networkUI.InitializeSubscribers();
            _scoreUIManager.InitializeSubscribers();
        }

        private void InitializeSubscribers()
        {
            GameManager.Instance.OnStartGame += GameManager_OnStartGame;
            GameManager.Instance.OnRestartGame += GameManager_OnRestartGame;
            GameManager.Instance.OnShowGameOverUI += GameManager_OnShowGameOverUI;
            Debug.Log($"[UIMNG] Subscribed");
        }

        private void GameManager_OnStartGame(object sender, EventArgs e)
        {
            ShowGameUI();
            HideStartGameUI();
        }

        private void GameManager_OnRestartGame(object sender, EventArgs e)
        {
            ShowGameUI();
            HideGameOverUI();
        }

        private void GameManager_OnShowGameOverUI(object sender, EventArgs e)
        {
            HideGameUI();
            ShowGameOverUI();
        }

        private void HideStartGameUI()
        {
            StartGameUIManager.Instance.Hide();
        }

        public void ShowGameUI()
        {
            ScoreUIManager.Instance.Show();
            HealthUIManager.Instance.Show();
        }

        public void HideGameUI()
        {
            ScoreUIManager.Instance.Hide();
            HealthUIManager.Instance.Hide();
        }

        public void ShowGameOverUI()
        {
            GameOverUIManager.Instance.Show();
        }

        public void HideGameOverUI()
        {
            GameOverUIManager.Instance.Hide();
        }
    }
}