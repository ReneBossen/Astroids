using Mirror;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public class ObjectPoolHandler : NetworkBehaviour
    {
        public static ObjectPoolHandler Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }

            Instance = this;
            DontDestroyOnLoad(this);
        }

        [Server]
        public async Task<List<GameObject>> InstantiateObjectPool(GameObject prefab, GameObject parent, int amount)
        {
            return await CreateObjectPool(prefab, parent, amount);
        }

        [Server]
        public async Task<List<GameObject>> InstantiateRandomObjectPoolByList(List<GameObject> prefabs, GameObject parent, int amount)
        {
            return await CreateRandomObjectPoolByList(prefabs, parent, amount);
        }

        [Server]
        private async Task<List<GameObject>> CreateObjectPool(GameObject prefab, GameObject parent, int amount)
        {
            List<GameObject> spawnedPrefabs = new();
            Vector2 spawnPosition = new(100, 100);

            Debug.Log($"[OPH] prefab: {prefab.name}, parent: {parent.name}");
            for (int i = 0; i < amount; i++)
            {
                GameObject tempPrefab = Instantiate(prefab, spawnPosition, Quaternion.identity, parent.transform);
                NetworkServer.Spawn(tempPrefab);
                spawnedPrefabs.Add(tempPrefab);

                StartCoroutine(GameObjectHandler.DisableAfterTime(tempPrefab, 0f));

                await Task.Yield();
            }

            return spawnedPrefabs;
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
                NetworkServer.Spawn(tempPrefab);
                spawnedPrefabs.Add(tempPrefab);

                StartCoroutine(GameObjectHandler.DisableAfterTime(tempPrefab, 0f));

                await Task.Yield();
            }

            return spawnedPrefabs;
        }
    }
}