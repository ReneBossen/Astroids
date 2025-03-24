using UnityEngine;

namespace Assets.Scripts.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        public static MainMenuUI Instance { get; private set; }

        [SerializeField] private NetworkUI _networkUI;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);
        }

        public void InitializeUISubscribers()
        {
            _networkUI.InitializeSubscribers();
        }
    }
}