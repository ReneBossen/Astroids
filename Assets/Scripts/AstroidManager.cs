using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class AstroidManager : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _astroidPrefabs;
        [SerializeField] private GameObject _explosionPrefab;

        private readonly List<GameObject> _spawnedAstroids = new();
        private readonly List<GameObject> _explosionPrefabs = new();

        private Camera _camera;
        private Vector2 _screenBounds;
        private GameObject _explosionParent;

        private void Awake()
        {
            _camera = Camera.main;
        }

        private void Start()
        {
            _explosionParent = new GameObject("ExplosionParent");
            InstantiateExplosionEffects(5);

            SpawnAstroids(5);
        }

        private void SpawnAstroids(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                GameObject spawnedAstroid = Instantiate(GetRandomAstroidPrefab(), GetRandomSpawnPosition(), Quaternion.identity);
                _spawnedAstroids.Add(spawnedAstroid);
                spawnedAstroid.GetComponent<Astroid>().OnAstroidHit += HandleAstroidHit;
            }
        }

        private void InstantiateExplosionEffects(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                GameObject explosionPrefab = Instantiate(_explosionPrefab, transform.position, Quaternion.identity, _explosionParent.transform);
                _explosionPrefabs.Add(explosionPrefab);

                StartCoroutine(DisableAfterTime(explosionPrefab, 0f));
            }
        }

        private void HandleAstroidHit(object sender, Astroid.OnAstroidHitEventArgs e)
        {
            SpawnExplosion();
        }

        private void SpawnExplosion()
        {
            GameObject explosion = _explosionPrefabs.FirstOrDefault(explosion => !explosion.activeInHierarchy);

            if (explosion != null)
            {
                explosion.SetActive(true);
                StartCoroutine(DisableAfterTime(explosion, 1f));
            }
            else
            {
                InstantiateExplosionEffects(5);
                SpawnExplosion();
            }
        }

        private GameObject GetRandomAstroidPrefab()
        {
            return _astroidPrefabs[Random.Range(0, _astroidPrefabs.Count)];
        }

        private Vector3 GetRandomSpawnPosition()
        {
            float randomX = Random.Range(-_screenBounds.x / 2, _screenBounds.x / 2);
            float randomY = Random.Range(-_screenBounds.y / 2, _screenBounds.y / 2);
            return new Vector3(randomX, randomY, 0);
        }

        private IEnumerator DisableAfterTime(GameObject prefab, float time)
        {
            yield return new WaitForSeconds(time);
            prefab.SetActive(false);
        }
    }
}