using System.Collections;
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

        private void Start()
        {
            StartCoroutine(WaitForScore());
        }

        private IEnumerator WaitForScore()
        {
            yield return new WaitUntil(() => Score.Instance != null);
            Score.Instance.OnScoreUpdated += Score_OnScoreUpdated;
        }

        private void Score_OnScoreUpdated(int score)
        {
            _scoreText.text = $"Score: {score}";
        }
    }
}