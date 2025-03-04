using Assets.Scripts.Helpers;
using Assets.Scripts.Network;
using Mirror;
using System;
using System.Collections.Generic;
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

        private Camera _camera;
        private Vector2 _screenBounds;
        private Queue<GameObject> _astroidQueue;
        //private Queue<GameObject> _explosionPrefabs = new();
        private List<GameObject> _spawnedAstroids = new();

        private bool _astroidQueueReady = false;
        private LevelManager.OnLevelStartedEventArgs _pendingLevelStartedArgs;

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

        public override void OnStartServer()
        {
            LevelManager.Instance.OnLevelStarted += LevelManager_OnLevelStarted;
            GameManager.Instance.OnGameOver += GameManager_OnGameOver;
            ObjectPoolHandler.Instance.OnAstroidQueueCreated += ObjectPoolHandler_OnAstroidQueueCreated;
        }

        private void ObjectPoolHandler_OnAstroidQueueCreated(object sender, EventArgs e)
        {
            _astroidQueue = ObjectPoolHandler.Instance.AstroidQueue;
            _astroidQueueReady = true;

            if (_pendingLevelStartedArgs != null)
            {
                LevelManager_OnLevelStarted(this, _pendingLevelStartedArgs);
                _pendingLevelStartedArgs = null;
            }
        }

        private void LevelManager_OnLevelStarted(object sender, LevelManager.OnLevelStartedEventArgs args)
        {
            if (!_astroidQueueReady)
            {
                Debug.Log($"[ASTMNG] AstroidQueue not ready. Waiting for queue to be ready");
                _pendingLevelStartedArgs = args;
                return;
            }

            Debug.Log($"[ASTMNG] AstroidQueue Count: {_astroidQueue.Count}");
            int astroidsToSpawn = args.AstroidsRemaining;

            SpawnAstroids(astroidsToSpawn);
        }

        [Server]
        public void SpawnAstroids(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                GameObject astroid = _astroidQueue.Dequeue();

                if (astroid == null)
                    return;

                Vector3 spawnPosition = GetRandomSpawnPosition();
                astroid.transform.position = spawnPosition;
                RepositionAstroidRpc(astroid, spawnPosition);

                _spawnedAstroids.Add(astroid);

                astroid.GetComponent<VariableSync>().IsActive = true;

                astroid.GetComponent<Astroid>().OnAstroidHit += Astroid_HandleAstroidHit;
                astroid.GetComponent<Astroid>().OnPlayerHit += Astroid_HandleAstroidHitPlayer;
            }
        }

        [ClientRpc]
        private void RepositionAstroidRpc(GameObject astroid, Vector3 position)
        {
            astroid.transform.position = position;
        }

        private void GameManager_OnGameOver(object sender, EventArgs e)
        {
            DestroyAllRemainingAstroids();
        }

        [Server]
        private void Astroid_HandleAstroidHitPlayer(object sender, Astroid.OnAstroidHitEventArgs astroid)
        {
            OnPlayerHitByAstroid?.Invoke(this, EventArgs.Empty);

            AstroidHit(astroid.Astroid, astroid.Value);
        }

        [Server]
        private void Astroid_HandleAstroidHit(object sender, Astroid.OnAstroidHitEventArgs astroid)
        {
            AstroidHit(astroid.Astroid, astroid.Value);
        }

        [Server]
        private void AstroidHit(GameObject astroid, int score)
        {
            //SpawnExplosion(astroid.Astroid);

            LevelManager.Instance.AsteroidDestroyed();

            OnAstroidDestroyed?.Invoke(this, new OnAstroidDestroyedEventArgs
            {
                score = score
            });

            Astroid astroidScript = astroid.GetComponent<Astroid>();

            astroidScript.OnAstroidHit -= Astroid_HandleAstroidHit;
            astroidScript.OnPlayerHit -= Astroid_HandleAstroidHitPlayer;

            _spawnedAstroids.Remove(astroid);

            astroid.GetComponent<VariableSync>().IsActive = false;
        }

        [Server]
        private void DestroyAllRemainingAstroids()
        {
            _spawnedAstroids.ForEach(astroid =>
            {
                astroid.GetComponent<Astroid>().OnAstroidHit -= Astroid_HandleAstroidHit;
                astroid.GetComponent<Astroid>().OnPlayerHit -= Astroid_HandleAstroidHitPlayer;
                astroid.GetComponent<VariableSync>().IsActive = false;
            });
        }

        //private void SpawnExplosion(GameObject astroid)
        //{
        //    GameObject explosion = _explosionPrefabs.FirstOrDefault(explosion => !explosion.activeInHierarchy);

        //    if (explosion == null)
        //        return;

        //    //GameObjectHandler.Instance.RepositionGameObject(explosion, astroid.transform.position);

        //    explosion.SetActive(true);
        //    explosion.GetComponent<ParticleSystem>().Play();

        //    gameObject.SetActive(false); //Should be deactivated after 1 sec
        //}

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