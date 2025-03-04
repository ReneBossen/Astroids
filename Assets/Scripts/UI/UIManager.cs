using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

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

            StartCoroutine(WaitForGameManager());
        }

        private IEnumerator WaitForGameManager()
        {
            yield return new WaitUntil(() => GameManager.Instance != null);
            GameManager.Instance.OnStartGame += GameManager_OnStartGame;
            GameManager.Instance.OnRestartGame += GameManager_OnRestartGame;
            GameManager.Instance.OnShowGameOverUI += GameManager_OnShowGameOverUI;
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