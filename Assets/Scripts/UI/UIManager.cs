using UnityEngine;

namespace Assets.Scripts.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

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
            ScoreUIManager.Instance.Show();
            HealthUIManager.Instance.Show();
        }
    }
}