using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class QuitMenuUI : MonoBehaviour
    {
        [SerializeField] private Button _quitButton;

        private void Start()
        {
            _quitButton.onClick.AddListener(() =>
            {
                Application.Quit();
            });
        }
    }
}