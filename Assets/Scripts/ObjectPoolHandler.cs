using Assets.Scripts.Network;
using Mirror;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    public class ObjectPoolHandler : NetworkBehaviour
    {
        public static ObjectPoolHandler Instance { get; private set; }

        public event EventHandler OnAstroidQueueCreated;
        public event EventHandler OnExplosionQueueCreated;
        public event EventHandler OnBulletQueueCreated;

        [SerializeField] private GameObject _emptyParent;

        [Header("Bullet Pool")]
        [SerializeField] private GameObject _bulletPrefab;

        [Header("Astroid Pool")]
        [SerializeField] private List<GameObject> _astroidPrefabs;

        [Header("Explosions")]
        [SerializeField] private GameObject _explosionPrefab;

        public Queue<GameObject> BulletQueue { get; } = new();
        public Queue<GameObject> AstroidQueue { get; } = new();
        public Queue<GameObject> ExplosionQueue { get; } = new();

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            Instance = this;
        }

        public override void OnStartServer()
        {
            GameManager.Instance.OnStartGame += GameManager_OnStartGame;
        }

        [Server]
        private async void GameManager_OnStartGame(object sender, EventArgs e)
        {
            await CreateBulletQueue(60);
            await CreateAstroidQueue(60);
            await CreateExplosionQueue(15);
        }

        [Server]
        private async Task CreateBulletQueue(int amount)
        {
            GameObject bulletPool = Instantiate(_emptyParent, Vector3.zero, Quaternion.identity);
            bulletPool.TryGetComponent(out NameSync bulletNameSync);
            bulletNameSync.objectName = "BulletPool";

            NetworkServer.Spawn(bulletPool);

            for (int i = 0; i < amount; i++)
            {
                InstantiateObjects(_bulletPrefab, BulletQueue, bulletPool);
            }

            BulletQueueNotifyClients();

            await Task.Yield();
        }

        [ClientRpc]
        private void BulletQueueNotifyClients()
        {
            OnBulletQueueCreated?.Invoke(this, EventArgs.Empty);
        }

        [Server]
        private async Task CreateAstroidQueue(int amount)
        {
            GameObject astroidPool = Instantiate(_emptyParent, Vector3.zero, Quaternion.identity);
            astroidPool.TryGetComponent(out NameSync astroidNameSync);
            astroidNameSync.objectName = "AstroidPool";

            NetworkServer.Spawn(astroidPool);

            for (int i = 0; i < amount; i++)
            {
                GameObject randomAstroid = _astroidPrefabs[Random.Range(0, _astroidPrefabs.Count)];
                InstantiateObjects(randomAstroid, AstroidQueue, astroidPool);
            }

            OnAstroidQueueCreated?.Invoke(this, EventArgs.Empty);

            await Task.Yield();
        }

        [Server]
        private async Task CreateExplosionQueue(int amount)
        {
            GameObject explosionPool = Instantiate(_emptyParent, Vector3.zero, Quaternion.identity);
            explosionPool.TryGetComponent(out NameSync explosionNameSync);
            explosionNameSync.objectName = "ExplosionPool";

            NetworkServer.Spawn(explosionPool);

            for (int i = 0; i < amount; i++)
            {
                InstantiateObjects(_explosionPrefab, ExplosionQueue, explosionPool);
            }

            OnExplosionQueueCreated?.Invoke(this, EventArgs.Empty);

            await Task.Yield();
        }

        [Server]
        private void InstantiateObjects(GameObject obj, Queue<GameObject> queue, GameObject parent)
        {
            GameObject tempObject = Instantiate(obj, Vector3.zero, Quaternion.identity);
            tempObject.TryGetComponent(out VariableSync sync);
            sync.IsActive = true;
            queue.Enqueue(tempObject);

            NetworkServer.Spawn(tempObject);

            SetObjectParentRpc(tempObject.transform, parent.transform);

            sync.IsActive = false;
        }

        [ClientRpc]
        private void SetObjectParentRpc(Transform childTransform, Transform parentTransform)
        {
            childTransform.SetParent(parentTransform);
        }
    }
}