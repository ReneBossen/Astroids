using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts
{
    public class Player : MonoBehaviour
    {
        private PlayerInput _playerInput;
        private Weapon _weapon;
        private PlayerMovement _playerMovement;

        private void Awake()
        {
            _playerInput = new PlayerInput();
        }

        private void Start()
        {
            _weapon = GetComponent<Weapon>();
            _playerMovement = GetComponent<PlayerMovement>();
        }
    }
}