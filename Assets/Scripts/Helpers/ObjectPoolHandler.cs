using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public class ObjectPoolHandler : MonoBehaviour
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

        public async Task<List<GameObject>> InstantiateObjectPool(GameObject prefab, GameObject parent, int amount)
        {
            return await CreateObjectPool(prefab, parent, amount);
        }

        private async Task<List<GameObject>> CreateObjectPool(GameObject prefab, GameObject parent, int amount)
        {
            List<GameObject> spawnedPrefabs = new();

            for (int i = 0; i < amount; i++)
            {
                GameObject tempPrefab = Instantiate(prefab, transform.position, Quaternion.identity, parent.transform);
                spawnedPrefabs.Add(tempPrefab);

                StartCoroutine(GameObjectHandler.DisableAfterTime(tempPrefab, 0f));

                await Task.Yield();
            }

            return spawnedPrefabs;
        }
    }
}