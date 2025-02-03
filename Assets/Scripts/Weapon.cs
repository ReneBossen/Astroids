using Assets.Scripts.Helpers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private GameObject _bulletPrefab;
        [SerializeField] private Transform _bulletSpawnPoint;
        [SerializeField] private float _lifeTimer;

        private readonly List<GameObject> _spawnedBulletPrefabs = new();

        private PlayerInput _playerInput;
        private GameObject _bulletPool;

        private void Awake()
        {
            _playerInput = new PlayerInput();
        }

        private void Start()
        {
            _bulletPool = new GameObject("BulletPool");
            InstantiateExplosionEffects(30);
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
            GameObject bullet = _spawnedBulletPrefabs.FirstOrDefault(explosion => !explosion.activeInHierarchy);

            if (bullet == null)
                return;

            GameObjectHandler.RepositionGameObject(bullet, _bulletSpawnPoint.transform.position);
            bullet.transform.rotation = transform.rotation;

            //TODO: Fix bullets disappearing to early if they hit an astroid, and get activated again

            bullet.SetActive(true);

            StartCoroutine(GameObjectHandler.DisableAfterTime(bullet, _lifeTimer));
        }

        private async void InstantiateExplosionEffects(int amount)
        {
            _spawnedBulletPrefabs.AddRange(
                await ObjectPoolHandler.Instance.InstantiateObjectPool(_bulletPrefab, _bulletPool, amount));
        }
    }
}