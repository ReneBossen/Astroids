using Assets.Scripts.GameCriticals;
using Mirror;
using System;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class Player : NetworkBehaviour
    {
        private PlayerMovement _playerMovement;

        private void Start()
        {
            _playerMovement = GetComponent<PlayerMovement>();

            GameManager.Instance.OnStartGame += GameManager_OnStartGame;
            GameManager.Instance.OnRestartGame += GameManager_OnRestartGame;
            GameManager.Instance.OnGameOver += GameManager_OnGameOver;
        }

        private void GameManager_OnStartGame(object sender, EventArgs e)
        {
            ResetPlayerPosition();
            gameObject.SetActive(true);
            _playerMovement.EnableMovement();
        }

        private void GameManager_OnRestartGame(object sender, EventArgs e)
        {
            ResetPlayerPosition();
            gameObject.SetActive(true);
            _playerMovement.EnableMovement();
        }

        private void GameManager_OnGameOver(object sender, EventArgs e)
        {
            gameObject.SetActive(false);
            _playerMovement.DisableMovement();
        }

        private void ResetPlayerPosition()
        {
            gameObject.transform.position = Vector3.zero;
            gameObject.transform.rotation = Quaternion.identity;
        }
    }
}