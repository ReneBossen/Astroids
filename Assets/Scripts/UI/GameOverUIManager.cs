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
        //TODO: GameOver Score and HighScore
        private void GameManager_OnGameOver(object sender, EventArgs e)
        {
            //_score =
        }
    }
}