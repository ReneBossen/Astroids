using Assets.Scripts.Helpers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using PlayerInput = Assets.Scripts.Player.PlayerInput;

namespace Assets.Scripts.Weapon
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private GameObject _bulletPrefab;
        [SerializeField] private Transform _bulletSpawnPoint;
        [SerializeField] private float _lifeTimer;

        private readonly List<GameObject> _spawnedBulletPrefabs = new();
        private readonly Dictionary<GameObject, Coroutine> _activeCoroutines = new();

        private PlayerInput _playerInput;
        private GameObject _bulletPool;

        private void Awake()
        {
            _playerInput = new PlayerInput();
        }

        private void Start()
        {
            _bulletPool = new GameObject("BulletPool");
            InstantiateBulletPrefabs(30);
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
            GameObject bullet = _spawnedBulletPrefabs.FirstOrDefault(bullet => !bullet.activeInHierarchy);

            if (bullet == null)
                return;

            GameObjectHandler.RepositionGameObject(bullet, _bulletSpawnPoint.transform.position);
            bullet.transform.rotation = transform.rotation;

            bullet.SetActive(true);

            if (_activeCoroutines.TryGetValue(bullet, out Coroutine oldCoroutine)
                && oldCoroutine != null)
            {
                StopCoroutine(oldCoroutine);
            }

            Coroutine newCoroutine = StartCoroutine(GameObjectHandler.DisableAfterTime(bullet, _lifeTimer));
            _activeCoroutines[bullet] = newCoroutine;
        }

        private void OnBulletHit(object sender, Bullet.BulletHitEventArgs args)
        {
            GameObject bullet = args.Bullet;
            if (_activeCoroutines.TryGetValue(bullet, out Coroutine coroutine)
                && coroutine != null)
            {
                StopCoroutine(coroutine);
                _activeCoroutines.Remove(bullet);
            }

            bullet.SetActive(false);
        }

        private async void InstantiateBulletPrefabs(int amount)
        {
            _spawnedBulletPrefabs.AddRange(
                await ObjectPoolHandler.Instance.InstantiateObjectPool(_bulletPrefab, _bulletPool, amount));

            _spawnedBulletPrefabs.ForEach(bullet => bullet.GetComponent<Bullet>().OnBulletHit += OnBulletHit);
        }
    }
}