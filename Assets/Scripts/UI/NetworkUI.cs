using Assets.Scripts.GameCriticals;
using Assets.Scripts.Network;
using System;
using TMPro;
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
                Debug.Log($"Unity Login");
                await _networkManager.UnityLogin();

                Debug.Log($"Creating Relay");
                await _networkManager.StartRelayHost(2);

                Debug.Log($"Displaying Code");
                DisplayCode();

                OnHostGame?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error creating Relay: {ex.Message}");
            }
        }

        private void DisplayCode()
        {
            _startHostButton.gameObject.SetActive(false);
            _startClientButton.gameObject.SetActive(false);
            _codeText.gameObject.SetActive(true);
            _joinInput.gameObject.SetActive(false);

            _codeText.text = $"Code: {_networkManager.relayJoinCode}";
        }

        private async void JoinRelay(string joinCode)
        {
            _networkManager = FindFirstObjectByType<AstroidsNetworkManager>();
            try
            {
                await _networkManager.UnityLogin();

                _networkManager.relayJoinCode = joinCode;

                _networkManager.JoinRelayServer();

                Hide();
            }
            catch (RelayServiceException ex)
            {
                Debug.LogError($"Relay join failed: {ex.Message}");
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
    }
}