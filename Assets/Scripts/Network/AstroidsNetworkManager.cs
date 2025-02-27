using Assets.Scripts.UI;
using Mirror;
using Mirror.Examples.Common.Controllers.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using Utp;

namespace Assets.Scripts.Network
{
    public class AstroidsNetworkManager : RelayNetworkManager
    {
        public bool IsLoggedIn = false;

        /// <summary>
        /// List of players currently connected to the server.
        /// </summary>
        public readonly List<Player.Player> Players = new();

        ///<summary>
        /// List of players currently connected to the server.
        /// </summary>
        public readonly List<NetworkConnectionToClient> WaitingPlayers = new();

        public override void Start()
        {
            StartGameUIManager.Instance.OnStartGame += StartGameUIManager_OnStartGame;
        }

        [Server]
        private void StartGameUIManager_OnStartGame(object sender, EventArgs e)
        {
            GameManager.Instance.ServerStartGame();
        }

        public async Task UnityLogin()
        {
            try
            {
                await UnityServices.InitializeAsync();
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log($"Logged into Unity, player ID: {AuthenticationService.Instance.PlayerId}");
                IsLoggedIn = true;
            }
            catch (Exception e)
            {
                IsLoggedIn = false;
                Debug.Log(e);
            }
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            base.OnServerDisconnect(conn);

            Dictionary<uint, NetworkIdentity> spawnedPlayers = NetworkServer.spawned;

            // Update players list on client disconnect
            foreach (Player.Player player in Players)
            {
                bool playerFound = spawnedPlayers
                    .Select(kvp => kvp.Value.GetComponent<Player.Player>())
                    .Any(playerComponent => playerComponent != null && player == playerComponent);

                if (playerFound)
                    continue;

                Players.Remove(player);
                break;
            }
        }

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            Debug.Log($"[NM] Client joined with: {conn.connectionId}");
            if (!GameManager.Instance.GameIsRunning)
            {
                WaitingPlayers.Add(conn);
                Debug.Log($"[NM] Player {conn.connectionId} added to waiting list!");
                return;
            }

            SpawnPlayer(conn);

            //foreach (Player.Player playerComponent in NetworkServer.spawned
            //             .Select(kvp => kvp.Value.GetComponent<Player.Player>())
            //             .Where(player => player != null && !Players.Contains(player)))
            //{
            //    Players.Add(playerComponent);
            //}
        }

        [Server]
        public void SpawnWaitingPlayers()
        {
            Debug.Log($"[NM] Spawning all waiting players: {GameManager.Instance.GameIsRunning}");
            if (GameManager.Instance.GameIsRunning)
                return;

            GameManager.Instance.GameIsRunning = true;

            foreach (NetworkConnectionToClient conn in WaitingPlayers)
            {
                Debug.Log($"[NM] Spawning waiting player: {conn.connectionId}");
                SpawnPlayer(conn);
            }

            WaitingPlayers.Clear();
        }

        [Server]
        private void SpawnPlayer(NetworkConnectionToClient conn)
        {
            GameObject player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
            NetworkServer.AddPlayerForConnection(conn, player);
            Debug.Log($"[NM] Added player for connection {conn.connectionId}");
        }
    }
}