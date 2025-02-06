using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Score
{
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _scoreText;

        private int _score;

        private void Start()
        {
            _score = 0;
            UpdateScoreText();

            AstroidManager.Instance.OnAstroidDestroyed += OnAstroidDestroyed;

            Hide();
        }

        private void OnAstroidDestroyed(object sender, AstroidManager.OnAstroidDestroyedEventArgs e)
        {
            AddScore(e.score);
        }

        private void AddScore(int amount)
        {
            _score += amount;

            UpdateScoreText();
        }

        private void UpdateScoreText()
        {
            _scoreText.text = $"Score: {_score}";
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}