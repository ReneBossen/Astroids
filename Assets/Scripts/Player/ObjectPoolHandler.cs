using Assets.Scripts.Network;
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

        public event EventHandler OnAstroidQueueCreated;

        [SerializeField] private GameObject _emptyParent;

        [Header("Bullet Pool")]
        [SerializeField] private GameObject _bulletPrefab;

        [Header("Astroid Pool")]
        [SerializeField] private List<GameObject> _astroidPrefabs;

        [Header("Explosions")]
        [SerializeField] private GameObject _explosionPrefab;

        public Queue<GameObject> BulletQueue { get; } = new();
        public Queue<GameObject> AstroidQueue { get; } = new();

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
        private void GameManager_OnStartGame(object sender, EventArgs e)
        {
            CreateBulletQueue(60);
            CreateAstroidQueue(60);
        }

        [Server]
        private void CreateBulletQueue(int amount)
        {
            GameObject bulletPool = Instantiate(_emptyParent, Vector3.zero, Quaternion.identity);
            bulletPool.TryGetComponent(out NameSync bulletNameSync);
            bulletNameSync.objectName = "BulletPool";

            NetworkServer.Spawn(bulletPool);

            for (int i = 0; i < amount; i++)
            {
                InstantiateObjects(_bulletPrefab, BulletQueue, bulletPool);
            }
        }

        [Server]
        private void CreateAstroidQueue(int amount)
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