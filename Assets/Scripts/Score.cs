using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts
{
    public class Score : MonoBehaviour
    {
        public static Score Instance { get; private set; }

        public int CurrentScore { get; private set; }

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
            CurrentScore = 0;
            ScoreUIManager.Instance.UpdateScoreText(CurrentScore);

            AstroidManager.Instance.OnAstroidDestroyed += AstroidManager_OnAstroidDestroyed;
            GameManager.Instance.OnRestartGame += GameManager_OnRestartGame;
        }

        private void GameManager_OnRestartGame(object sender, System.EventArgs e)
        {
            ResetScore();
        }

        private void AstroidManager_OnAstroidDestroyed(object sender, AstroidManager.OnAstroidDestroyedEventArgs e)
        {
            AddScore(e.score);
        }

        private void AddScore(int amount)
        {
            CurrentScore += amount;
            ScoreUIManager.Instance.UpdateScoreText(CurrentScore);
        }

        private void ResetScore()
        {
            CurrentScore = 0;
            ScoreUIManager.Instance.UpdateScoreText(CurrentScore);
        }
    }
}