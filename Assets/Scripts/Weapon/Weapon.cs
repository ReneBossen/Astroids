using Assets.Scripts.GameCriticals;
using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using PlayerInput = Assets.Scripts.Player.PlayerInput;

namespace Assets.Scripts.Weapon
{
    public class Weapon : NetworkBehaviour
    {
        public event EventHandler OnPlayerHitByBullet;

        [SerializeField] private Transform _bulletSpawnPoint;

        private Queue<GameObject> _bulletQueue;
        private PlayerInput _playerInput;

        private void Awake()
        {
            _playerInput = new PlayerInput();
        }

        private void Start()
        {
            ObjectPoolHandler.Instance.OnBulletQueueCreated += ObjectPoolHandler_OnBulletQueueCreated;
        }

        private void ObjectPoolHandler_OnBulletQueueCreated(object sender, EventArgs e)
        {
            _bulletQueue = ObjectPoolHandler.Instance.BulletQueue;
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
            if (!isLocalPlayer)
                return;

            CmdShoot();
        }

        [Command]
        private void CmdShoot()
        {
            GameObject bulletObject = _bulletQueue.Dequeue();
            Debug.Log($"[WPNSHOOT] bulletId: {bulletObject.GetComponent<Transform>().GetInstanceID()}");

            Vector3 spawnPosition = _bulletSpawnPoint.transform.position;
            bulletObject.transform.position = spawnPosition;
            bulletObject.transform.rotation = transform.rotation;

            Bullet bullet = bulletObject.GetComponent<Bullet>();

            bullet.OnPlayerHit += Bullet_HandlePlayerHit;
            bullet.OnDisable += Bullet_OnDisable;
            bullet.ShooterIdentity = gameObject.GetComponent<NetworkIdentity>();

            RepositionBulletRpc(bulletObject, spawnPosition, transform.rotation);

            if (bulletObject.TryGetComponent(out Bullet bulletScript))
                bulletScript.IsActive = true;

            _bulletQueue.Enqueue(bulletObject);
        }

        private void Bullet_OnDisable(object sender, Bullet.OnBulletDisableEventArgs bullet)
        {
            bullet.Bullet.GetComponent<Bullet>().OnPlayerHit -= Bullet_HandlePlayerHit;
            bullet.Bullet.GetComponent<Bullet>().OnDisable -= Bullet_OnDisable;
        }


        private void Bullet_HandlePlayerHit(object sender, Bullet.OnBulletDisableEventArgs bullet)
        {
            OnPlayerHitByBullet?.Invoke(this, EventArgs.Empty);

            bullet.Bullet.GetComponent<Bullet>().OnPlayerHit -= Bullet_HandlePlayerHit;
            bullet.Bullet.GetComponent<Bullet>().OnDisable -= Bullet_OnDisable;
        }

        [ClientRpc]
        private void RepositionBulletRpc(GameObject bullet, Vector3 position, Quaternion rotation)
        {
            bullet.transform.position = position;
            bullet.transform.rotation = rotation;
        }
    }
}