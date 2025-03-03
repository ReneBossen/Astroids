using Assets.Scripts.Weapon;
using Mirror;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;
using Task = System.Threading.Tasks.Task;

namespace Assets.Scripts.Helpers
{
    public class ObjectPoolHandler : NetworkBehaviour
    {
        public static ObjectPoolHandler Instance { get; private set; }

        public event EventHandler OnBulletsInstantiated;

        [Header("Bullet Pool")]
        [SerializeField] private GameObject _bulletPrefab;
        [SerializeField] private GameObject _bulletPool;

        public Queue<GameObject> BulletQueue { get; } = new();

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            Instance = this;
        }

        private void Start()
        {
            if (isServer)
            {
                GameManager.Instance.OnStartGame += GameManager_OnStartGame;
            }
        }

        [Server]
        private void GameManager_OnStartGame(object sender, EventArgs e)
        {
            CreateBulletQueue(60);
        }

        [Server]
        private void CreateBulletQueue(int amount)
        {
            GameObject bulletPool = Instantiate(_bulletPool, Vector3.zero, Quaternion.identity);
            bulletPool.TryGetComponent<NameSync>(out NameSync bulletNameSync);
            bulletNameSync.objectName = "BulletPool";

            NetworkServer.Spawn(bulletPool);

            for (int i = 0; i < amount; i++)
            {
                GameObject tempBullet = Instantiate(_bulletPrefab, Vector3.zero, Quaternion.identity);
                tempBullet.TryGetComponent(out Bullet bulletScript);
                bulletScript.IsActive = true;
                BulletQueue.Enqueue(tempBullet);

                NetworkServer.Spawn(tempBullet);

                SetObjectParentRpc(tempBullet.transform, bulletPool.transform);

                bulletScript.IsActive = false;
            }

            OnBulletsInstantiated?.Invoke(this, EventArgs.Empty);
        }

        [ClientRpc]
        private void SetObjectParentRpc(Transform childTransform, Transform parentTransform)
        {
            childTransform.SetParent(parentTransform);
        }

        [Server]
        public async Task<List<GameObject>> InstantiateRandomObjectPoolByList(List<GameObject> prefabs, GameObject parent, int amount)
        {
            return await CreateRandomObjectPoolByList(prefabs, parent, amount);
        }

        [Server]
        private async Task<List<GameObject>> CreateRandomObjectPoolByList(List<GameObject> prefabs, GameObject parent, int amount)
        {
            List<GameObject> spawnedPrefabs = new();
            Vector2 spawnPosition = new(50, 50);

            for (int i = 0; i < amount; i++)
            {
                GameObject randomPrefab = prefabs[Random.Range(0, prefabs.Count)];
                GameObject tempPrefab = Instantiate(randomPrefab, spawnPosition, Quaternion.identity, parent.transform);
                spawnedPrefabs.Add(tempPrefab);
                tempPrefab.SetActive(false);

                await Task.Yield();
            }

            return spawnedPrefabs;
        }
    }
}