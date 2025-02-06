using Assets.Scripts.Score;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [SerializeField] private StartGameManager _startGameManager;
        [SerializeField] private ScoreManager _scoreManager;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }

            Instance = this;
        }

        public void ShowScoreUI()
        {
            _scoreManager.Show();
        }
    }
}
