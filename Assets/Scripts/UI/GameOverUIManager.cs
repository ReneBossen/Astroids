using Assets.Scripts.Database;
using Assets.Scripts.GameCriticals;
using Mirror;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class GameOverUIManager : UIManagerBaseClass
    {
        public static GameOverUIManager Instance { get; private set; }

        public event EventHandler OnRestartGame;

        [SerializeField] private Button _restartBtn;
        [SerializeField] private TextMeshProUGUI _score;
        [SerializeField] private TextMeshProUGUI _highscore;

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
            _restartBtn.onClick.AddListener(() =>
            {
                OnRestartGame?.Invoke(this, EventArgs.Empty);
            });
        }

        public void InitializeSubscribers()
        {
            GameManager.Instance.OnShowGameOverUI += GameManager_OnShowGameOverUI;
            Debug.Log($"[GOUIMNG] subscribed");
        }

        private void GameManager_OnShowGameOverUI(object sender, EventArgs e)
        {
            if (!NetworkServer.active)
            {
                _restartBtn.gameObject.SetActive(false);
            }

            Debug.Log($"[GO_MNG] Final Score processed");

            LocalSave.TrySaveHighscore(Score.Instance.CurrentScore);

            _score.text = $"Score: {Score.Instance.CurrentScore}";
            _highscore.text = $"Highscore: {LocalSave.GetHighscore()}";
        }
    }
}