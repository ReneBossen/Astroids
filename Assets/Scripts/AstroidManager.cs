using Assets.Scripts.Helpers;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    public class AstroidManager : NetworkBehaviour
    {
        public static AstroidManager Instance { get; private set; }

        public event EventHandler OnPlayerHitByAstroid;
        public event EventHandler<OnAstroidDestroyedEventArgs> OnAstroidDestroyed;

        public class OnAstroidDestroyedEventArgs : EventArgs
        {
            public int score;
        }

        //[SerializeField] private List<GameObject> _astroidPrefabs;
        //[SerializeField] private GameObject _explosionPrefab;

        [SyncVar]
        private readonly List<GameObject> _instantiatedAstroids = new();
        [SyncVar]
        private readonly List<GameObject> _spawnedAstroids = new();
        [SyncVar]
        private readonly List<GameObject> _explosionPrefabs = new();

        private Camera _camera;
        private Vector2 _screenBounds;
        private GameObject _explosionPool;
        private GameObject _astroidPool;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }

            Instance = this;

            _camera = Camera.main;
            CalculateScreenBounds();
        }

        private void Start()
        {
            _explosionPool = new GameObject("ExplosionPool");
            _astroidPool = new GameObject("AstroidPool");
            if (isServer)
            {
                InstantiateExplosionEffects(10);

                _ = InstantiateAstroids(30);
            }

            LevelManager.Instance.OnLevelStarted += LevelManager_OnLevelStarted;
            GameManager.Instance.OnGameOver += GameManager_OnGameOver;
        }

        private void GameManager_OnGameOver(object sender, EventArgs e)
        {
            DestroyAllRemainingAstroids();
        }

        private void LevelManager_OnLevelStarted(object sender, LevelManager.OnLevelStartedEventArgs args)
        {
            int astroidsToSpawn = args.AstroidsRemaining;

            SpawnAstroids(astroidsToSpawn);
        }

        [Server]
        private async Task InstantiateAstroids(int amount)
        {
            //_instantiatedAstroids.AddRange(
            //    await ObjectPoolHandler.Instance.InstantiateRandomObjectPoolByList(_astroidPrefabs, _astroidPool, amount));
        }

        [Server]
        public async void SpawnAstroids(int amount)
        {
            if (amount > _instantiatedAstroids.Count)
            {
                await InstantiateAstroids(Mathf.Abs(amount - _instantiatedAstroids.Count));
            }

            PlaceAstroids(amount);
        }

        [Server]
        private void PlaceAstroids(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                GameObject astroid = _instantiatedAstroids.OrderBy(astroid => Random.Range(0, _instantiatedAstroids.Count)).FirstOrDefault(astroid => !astroid.activeInHierarchy);

                if (astroid == null)
                {
                    return;
                }

                astroid.transform.position = GetRandomSpawnPosition();
                _spawnedAstroids.Add(astroid);

                astroid.SetActive(true);

                astroid.GetComponent<Astroid>().OnAstroidHit += Astroid_HandleAstroidHit;
                astroid.GetComponent<Astroid>().OnPlayerHit += Astroid_HandleAstroidHitPlayer;
            }
        }

        [Server]
        private async void InstantiateExplosionEffects(int amount)
        {
            //_explosionPrefabs.AddRange(
            //    await ObjectPoolHandler.Instance.InstantiateObjectPool(_explosionPrefab, _explosionPool, amount));
        }

        private void Astroid_HandleAstroidHitPlayer(object sender, EventArgs e)
        {
            OnPlayerHitByAstroid?.Invoke(this, EventArgs.Empty);
        }

        private void Astroid_HandleAstroidHit(object sender, Astroid.OnAstroidHitEventArgs astroid)
        {
            SpawnExplosion(astroid.Astroid);

            LevelManager.Instance.AsteroidDestroyed();

            OnAstroidDestroyed?.Invoke(this, new OnAstroidDestroyedEventArgs
            {
                score = astroid.Value
            });

            astroid.Astroid.GetComponent<Astroid>().OnAstroidHit -= Astroid_HandleAstroidHit;
            astroid.Astroid.GetComponent<Astroid>().OnPlayerHit -= Astroid_HandleAstroidHitPlayer;

            _spawnedAstroids.Remove(astroid.Astroid);

            gameObject.SetActive(false);
        }

        private void DestroyAllRemainingAstroids()
        {
            _spawnedAstroids.ForEach(astroid =>
            {
                astroid.GetComponent<Astroid>().OnAstroidHit -= Astroid_HandleAstroidHit;
                astroid.GetComponent<Astroid>().OnPlayerHit -= Astroid_HandleAstroidHitPlayer;
                gameObject.SetActive(false);
            });
        }

        private void SpawnExplosion(GameObject astroid)
        {
            GameObject explosion = _explosionPrefabs.FirstOrDefault(explosion => !explosion.activeInHierarchy);

            if (explosion == null)
                return;

            //GameObjectHandler.Instance.RepositionGameObject(explosion, astroid.transform.position);

            explosion.SetActive(true);
            explosion.GetComponent<ParticleSystem>().Play();

            gameObject.SetActive(false); //Should be deactivated after 1 sec
        }

        private Vector3 GetRandomSpawnPosition()
        {
            List<GameObject> players = GameManager.Instance.Players;
            Vector3 spawnPosition = GenerateRandomPositionOnScreen();
            const float spawnDistanceFromPlayer = 2.5f;

            foreach (GameObject player in players)
            {
                while (Vector3.Distance(player.transform.position, spawnPosition) < spawnDistanceFromPlayer)
                {
                    spawnPosition = GenerateRandomPositionOnScreen();
                }
            }

            return spawnPosition;
        }

        private Vector3 GenerateRandomPositionOnScreen()
        {
            float randomX = Random.Range(-_screenBounds.x / 2, _screenBounds.x / 2);
            float randomY = Random.Range(-_screenBounds.y / 2, _screenBounds.y / 2);
            Vector3 position = new(randomX, randomY, 0);

            return position;
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