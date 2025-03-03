using Assets.Scripts.Helpers;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using PlayerInput = Assets.Scripts.Player.PlayerInput;

namespace Assets.Scripts.Weapon
{
    public class Weapon : NetworkBehaviour
    {
        [SerializeField] private Transform _bulletSpawnPoint;

        private Queue<GameObject> _bulletQueue;
        private PlayerInput _playerInput;

        private void Awake()
        {
            _playerInput = new PlayerInput();
        }

        private void OnEnable()
        {
            _playerInput.Enable();
            _playerInput.Player.Shoot.performed += OnShoot;
            _bulletQueue = ObjectPoolHandler.Instance.BulletQueue;
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
            GameObject bullet = _bulletQueue.Dequeue();

            Vector3 spawnPosition = _bulletSpawnPoint.transform.position;
            bullet.transform.position = spawnPosition;
            bullet.transform.rotation = transform.rotation;

            RepositionBulletRpc(bullet, spawnPosition, transform.rotation);

            if (bullet.TryGetComponent(out Bullet bulletScript))
                bulletScript.IsActive = true;

            _bulletQueue.Enqueue(bullet);

            Debug.Log($"[WPNCMD] BulletQueue Count: {_bulletQueue.Count}");
        }

        [ClientRpc]
        private void RepositionBulletRpc(GameObject bullet, Vector3 position, Quaternion rotation)
        {
            bullet.transform.position = position;
            bullet.transform.rotation = rotation;
        }
    }
}