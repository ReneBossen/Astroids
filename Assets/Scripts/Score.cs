using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts
{
    public class Score : MonoBehaviour
    {
        private int _score;

        private void Start()
        {
            _score = 0;
            ScoreUIManager.Instance.UpdateScoreText(_score);

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
            _score += amount;
            ScoreUIManager.Instance.UpdateScoreText(_score);
        }

        private void ResetScore()
        {
            _score = 0;
            ScoreUIManager.Instance.UpdateScoreText(_score);
        }
    }
}