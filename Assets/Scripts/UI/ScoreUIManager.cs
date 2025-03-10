using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class ScoreUIManager : UIManagerBaseClass
    {
        public static ScoreUIManager Instance { get; private set; }

        [SerializeField] private TextMeshProUGUI _scoreText;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            Instance = this;
        }

        public void UpdateScoreText(int score)
        {
            _scoreText.text = $"Score: {score}";
        }
    }
}