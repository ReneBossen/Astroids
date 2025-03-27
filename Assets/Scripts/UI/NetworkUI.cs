using Assets.Scripts.GameCriticals;
using Assets.Scripts.Network;
using System;
using System.Collections;
using System.Text.RegularExpressions;
using TMPro;
using Unity.Networking.Transport;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class NetworkUI : UIManagerBaseClass
    {
        public static NetworkUI Instance { get; private set; }

        public event EventHandler OnHostGame;

        [SerializeField] private Button _startHostButton;
        [SerializeField] private Button _startClientButton;
        [SerializeField] private TextMeshProUGUI _codeText;
        [SerializeField] private TMP_InputField _joinInput;
        [SerializeField] private TextMeshProUGUI _errorText;

        private AstroidsNetworkManager _networkManager;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            _startHostButton.onClick.AddListener(CreateRelay);
            _startClientButton.onClick.AddListener(() =>
            {
                JoinRelay(_joinInput.text);
            });
        }

        private void Start()
        {
            _networkManager = FindFirstObjectByType<AstroidsNetworkManager>();
        }

        private void OnEnable()
        {
            InitializeNetworkUI();
        }

        public void InitializeSubscribers()
        {
            GameManager.Instance.OnStartGame += GameManager_OnStartGame;
        }

        private void GameManager_OnStartGame(object sender, EventArgs e)
        {
            Hide();
        }

        private async void CreateRelay()
        {
            _networkManager = FindFirstObjectByType<AstroidsNetworkManager>();
            try
            {
                ShowHostScreen();

                await _networkManager.UnityLogin();

                await _networkManager.StartRelayHost(2);

                DisplayCode();
                OnHostGame?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error creating Relay: {ex.Message}");
            }
        }

        private void ShowHostScreen()
        {
            _codeText.text = "Code: ";

            _startHostButton.gameObject.SetActive(false);
            _startClientButton.gameObject.SetActive(false);
            _codeText.gameObject.SetActive(true);
            _joinInput.gameObject.SetActive(false);
        }
        private void DisplayCode()
        {
            _codeText.text = $"Code: {_networkManager.relayJoinCode}";
        }

        private async void JoinRelay(string joinCode)
        {
            if (!IsValidJoinCode(joinCode))
            {
                Debug.Log($"Join Code is incorrect");
                return;
            }

            _networkManager = FindFirstObjectByType<AstroidsNetworkManager>();
            try
            {
                Hide();
                await _networkManager.UnityLogin();

                _networkManager.relayJoinCode = joinCode;

                await _networkManager.JoinRelayServer();
            }
            catch (RelayServiceException ex)
            {
                Debug.LogError($"Relay join failed: {ex.Message}");
            }
            catch (Exception _)
            {
                _joinInput.text = "";
                Show();
                AuthenticationService.Instance.SignOut();
                StartCoroutine(DisplayError("Invalid JoinCode"));
            }
        }

        public void InitializeNetworkUI()
        {
            Show();
            _startHostButton.gameObject.SetActive(true);
            _startClientButton.gameObject.SetActive(true);
            _codeText.gameObject.SetActive(false);
            _joinInput.gameObject.SetActive(true);

            _joinInput.text = "";
        }

        private bool IsValidJoinCode(string joinCode)
        {
            Regex regex = new Regex(@"^[a-zA-Z0-9]{6}$");

            return !string.IsNullOrEmpty(joinCode) && regex.IsMatch(joinCode);
        }

        private IEnumerator DisplayError(string message)
        {
            _errorText.text = message;
            _errorText.gameObject.SetActive(true);
            yield return new WaitForSeconds(2);
            _errorText.gameObject.SetActive(false);
        }
    }
}