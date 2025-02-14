using Assets.Scripts.Network;
using Mirror;
using System;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class NetworkUI : MonoBehaviour
    {
        [SerializeField] private Button _startHostButton;
        [SerializeField] private Button _startClientButton;
        [SerializeField] private TextMeshProUGUI _codeText;
        [SerializeField] private TMP_InputField _joinInput;

        private AstroidsNetworkManager _networkManager;

        private void Awake()
        {
            _startHostButton.onClick.AddListener(CreateRelay);
            _startClientButton.onClick.AddListener(() =>
            {
                JoinRelay(_joinInput.text);
            });
        }

        private async void Start()
        {
            _networkManager = FindFirstObjectByType<AstroidsNetworkManager>();
            await UnityServices.InitializeAsync();

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        private async void CreateRelay()
        {
            try
            {
                await _networkManager.UnityLogin();

                await _networkManager.StartRelayHost(2);

                _codeText.text = $"Code: {_networkManager.relayJoinCode}";
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error creating Relay: {ex.Message}");
            }
        }

        private async void JoinRelay(string joinCode)
        {
            try
            {
                await _networkManager.UnityLogin();

                _networkManager.relayJoinCode = joinCode;

                _networkManager.JoinRelayServer();
            }
            catch (RelayServiceException ex)
            {
                Debug.LogError($"Relay join failed: {ex.Message}");
            }
        }
    }
}
