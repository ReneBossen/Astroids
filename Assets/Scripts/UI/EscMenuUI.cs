using Mirror;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using PlayerInput = Assets.Scripts.Player.PlayerInput;

namespace Assets.Scripts.UI
{
    public class EscMenuUI : MonoBehaviour
    {
        public static EscMenuUI Instance { get; private set; }

        public event EventHandler OnLeaveGame;

        [SerializeField] private GameObject EscMenu;
        [SerializeField] private Button _leaveButton;

        private PlayerInput _playerInput;

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

            _playerInput = new PlayerInput();
        }

        private void Start()
        {
            _leaveButton.onClick.AddListener(() =>
            {
                OnLeaveGame?.Invoke(this, EventArgs.Empty);

                LeaveGame();

                EscMenu.SetActive(false);
            });

            EscMenu.SetActive(false);
        }

        public void StartGameRpc()
        {
            _playerInput.Enable();
            _playerInput.Player.Esc.performed += ToggleMenu;
        }

        public void LeaveGame()
        {
            _playerInput.Player.Esc.performed -= ToggleMenu;
            _playerInput.Disable();
        }

        private void ToggleMenu(InputAction.CallbackContext callbackContext)
        {
            Debug.Log($"[ESC] ToggleMenu");
            EscMenu.SetActive(!EscMenu.activeSelf);
        }
    }
}