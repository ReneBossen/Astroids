using Assets.Scripts.GameCriticals;
using Assets.Scripts.UI;
using Assets.UTPTransport.Relay;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using Utp;

namespace Assets.Scripts.Network
{
    public class AstroidsNetworkManager : RelayNetworkManager
    {
        public bool IsLoggedIn = false;
        public static event EventHandler OnDisconnect;

        /// <summary>
        /// List of players currently connected to the server.
        /// </summary>
        public readonly List<Player.Player> Players = new();

        ///<summary>
        /// List of players currently waiting for game start.
        /// </summary>
        public readonly List<NetworkConnectionToClient> WaitingPlayers = new();

        public override void Start()
        {
            Debug.Log($"[ANM] Createdd new instance");
            base.Start();
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

        public override void OnClientDisconnect()
        {
            AuthenticationService.Instance.SignOut();
            OnDisconnect?.Invoke(this, EventArgs.Empty);
        }

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            Debug.Log($"[NM] Client joined with: {conn.connectionId}");
            if (!GameManager.Instance.GameIsRunning)
            {
                WaitingPlayers.Add(conn);
                return;
            }

            SpawnPlayer(conn);
        }

        [Server]
        public List<GameObject> SpawnWaitingPlayers()
        {
            if (GameManager.Instance.GameIsRunning)
                return null;

            GameManager.Instance.GameIsRunning = true;

            List<GameObject> players = new();

            foreach (NetworkConnectionToClient conn in WaitingPlayers)
            {
                Debug.Log($"[NM] Spawning waiting player: {conn.connectionId}");
                players.Add(SpawnPlayer(conn));
            }

            WaitingPlayers.Clear();

            return players;
        }

        [Server]
        private GameObject SpawnPlayer(NetworkConnectionToClient conn)
        {
            GameObject player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
            NetworkServer.AddPlayerForConnection(conn, player);

            return player;
        }
    }
}