using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class ScoreUIManager : MonoBehaviour
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

            Hide();
        }

        public void UpdateScoreText(int score)
        {
            _scoreText.text = $"Score: {score}";
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