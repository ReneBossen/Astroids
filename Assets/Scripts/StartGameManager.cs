using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class StartGameManager : MonoBehaviour
    {
        [SerializeField] private Button _readyButton;

        private void Start()
        {
            _readyButton.onClick.AddListener(() =>
            {
                LevelManager.Instance.StartGame();
                UIManager.Instance.ShowScoreUI();
                _readyButton.gameObject.SetActive(false);
            });
        }
    }
}