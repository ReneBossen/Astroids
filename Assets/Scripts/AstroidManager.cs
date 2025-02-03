using Assets.Scripts.Helpers;
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
        private GameObject _explosionPool;

        private void Awake()
        {
            _camera = Camera.main;
            CalculateScreenBounds();
        }

        private void Start()
        {
            _explosionPool = new GameObject("ExplosionPool");
            InstantiateExplosionEffects(10);

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

        private async void InstantiateExplosionEffects(int amount)
        {
            _explosionPrefabs.AddRange(
                await ObjectPoolHandler.Instance.InstantiateObjectPool(_explosionPrefab, _explosionPool, amount));
        }

        private void HandleAstroidHit(object sender, Astroid.OnAstroidHitEventArgs astroid)
        {
            SpawnExplosion(astroid.Astroid);

            StartCoroutine(GameObjectHandler.DisableAfterTime(astroid.Astroid, 0f));
        }

        private void SpawnExplosion(GameObject astroid)
        {
            GameObject explosion = _explosionPrefabs.FirstOrDefault(explosion => !explosion.activeInHierarchy);

            if (explosion == null)
                return;

            GameObjectHandler.RepositionGameObject(explosion, astroid.transform.position);

            explosion.SetActive(true);
            explosion.GetComponent<ParticleSystem>().Play();

            StartCoroutine(GameObjectHandler.DisableAfterTime(explosion, 1f));
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

        private void CalculateScreenBounds()
        {
            Vector3 screenBottomLeft = _camera.ViewportToWorldPoint(new Vector3(0, 0, 0));
            Vector3 screenTopRight = _camera.ViewportToWorldPoint(new Vector3(1, 1, 0));
            _screenBounds = new Vector2(
                screenTopRight.x - screenBottomLeft.x,
                screenTopRight.y - screenBottomLeft.y
            );
        }
    }
}