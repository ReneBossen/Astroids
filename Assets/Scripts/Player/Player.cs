using UnityEngine;

namespace Assets.Scripts.Player
{
    public class Player : MonoBehaviour
    {
        private PlayerInput _playerInput;
        private Weapon.Weapon _weapon;
        private PlayerMovement _playerMovement;

        private void Awake()
        {
            _playerInput = new PlayerInput();
        }

        private void Start()
        {
            _weapon = GetComponent<Weapon.Weapon>();
            _playerMovement = GetComponent<PlayerMovement>();
        }
    }
}