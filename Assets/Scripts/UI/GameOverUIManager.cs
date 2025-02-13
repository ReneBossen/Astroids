using Assets.Scripts.Database;
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

            GameManager.Instance.OnGameOver += GameManager_OnGameOver;
        }
        private void GameManager_OnGameOver(object sender, EventArgs e)
        {
            SetGameOverScore();
        }

        private void SetGameOverScore()
        {
            LocalSave.TrySaveHighscore(Score.Instance.CurrentScore);

            _score.text = $"Score: {Score.Instance.CurrentScore}";
            _highscore.text = $"Highscore: {LocalSave.GetHighscore()}";
        }
    }
}