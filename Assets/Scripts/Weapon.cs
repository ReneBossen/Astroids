using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private GameObject _bulletPrefab;
        [SerializeField] private Transform _bulletSpawnPoint;
        private PlayerInput _playerInput;

        private void Awake()
        {
            _playerInput = new PlayerInput();
        }

        private void OnEnable()
        {
            _playerInput.Enable();
            _playerInput.Player.Shoot.performed += OnShoot;
        }

        private void OnDisable()
        {
            _playerInput.Player.Shoot.performed -= OnShoot;
            _playerInput.Disable();
        }

        private void OnShoot(InputAction.CallbackContext obj)
        {
            GameObject bullet = Instantiate(_bulletPrefab, _bulletSpawnPoint.position, transform.rotation);
            Destroy(bullet, 2f);
        }
    }
}
