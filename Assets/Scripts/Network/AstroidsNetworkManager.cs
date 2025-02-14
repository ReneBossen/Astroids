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
        private readonly List<Player.Player> _players = new();

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
            foreach (Player.Player player in _players)
            {
                bool playerFound = spawnedPlayers
                    .Select(kvp => kvp.Value.GetComponent<Player.Player>())
                    .Any(playerComponent => playerComponent != null && player == playerComponent);

                if (playerFound)
                    continue;

                _players.Remove(player);
                break;
            }
        }

        public override void OnServerAddPlayer(NetworkConnectionToClient client)
        {
            base.OnServerAddPlayer(client);

            foreach (Player.Player playerComponent in NetworkServer.spawned
                         .Select(kvp => kvp.Value.GetComponent<Player.Player>())
                         .Where(player => player != null && !_players.Contains(player)))
            {
                _players.Add(playerComponent);
            }

        }
    }
}