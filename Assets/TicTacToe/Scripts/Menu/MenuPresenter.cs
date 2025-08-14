using Cysharp.Threading.Tasks;
using TicTacToe.Global;
using TMPro;
using UniRx;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TicTacToe.Networking
{
    public class MenuPresenter : MonoBehaviour
    {
        [SerializeField] private Button _hostButton;
        [SerializeField] private Button _quickJoinButton;
        [SerializeField] private TextMeshProUGUI _statusText;
        
        private NetworkManager _networkManager;
        private ITransportHandler _unityTransport;
        private IRelayHandler _relaySystem;
        private ILobbyHandler _lobbySystem;
        private readonly CompositeDisposable _disp = new();
        
        [Inject]
        public void Construct(NetworkManager network, ITransportHandler transport, IRelayHandler relay, ILobbyHandler lobby)
        {
            _networkManager = network;
            _unityTransport = transport;
            _relaySystem = relay;
            _lobbySystem = lobby;
        }
        
        private void Start()
        {
            Subscribe();
            SetStatus("...");
        }

        private void OnDestroy()
        {
            _disp.Dispose();
            UnSubscribe();
        }

        private void Subscribe()
        {
            _hostButton.OnClickAsObservable()
                .Subscribe(_ => StartHost().Forget())
                .AddTo(_disp);

            _quickJoinButton.OnClickAsObservable()
                .Subscribe(_ => StartJoin().Forget())
                .AddTo(_disp);

            _networkManager.OnClientConnectedCallback += OnClientConnected;
            _networkManager.OnClientDisconnectCallback += OnClientDisconnected;
        }

        private void UnSubscribe()
        {
            if (_networkManager && _networkManager.IsServer)
            {
                _networkManager.OnClientConnectedCallback -= OnClientConnected;
                _networkManager.OnClientDisconnectCallback -= OnClientDisconnected;
            }
        }
        
        private async UniTaskVoid StartHost()
        {
            try
            {
                SetStatus("Allocating Relay..");
                var (server, code) = await _relaySystem.AllocateAsync(1);
                _unityTransport.Configure(server);

                SetStatus($"Creating Lobby..");
                await _lobbySystem.CreateAsync("TicTacToeGame", 2, code);

                SetStatus("Starting Host..");
                if (_networkManager.StartHost())
                {
                    SetStatus($"HOSTED. CODE: {code}");
                }
                else
                {
                    SetStatus("StartHost failed.");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
                SetStatus("Error");
            }
        }

        private async UniTaskVoid StartJoin()
        {
            try
            {
                SetStatus("Quick Joining Lobby..");
                var lobby = await _lobbySystem.QuickJoinAsync();
                var code = lobby.Data["relayCode"].Value;

                SetStatus($"Joining Relay.. code={code}");
                var server = await _relaySystem.JoinAsync(code);
                _unityTransport.Configure(server);

                SetStatus("Starting Client..");
                if (_networkManager.StartClient())
                {
                    SetStatus("CONNECTED");
                }
                else
                    SetStatus("Connection failed");
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
                SetStatus("Error");
            }
        }

        private void OnClientConnected(ulong clientId)
        {
            SetStatus($"Client connected: {clientId}");
            if (clientId != _networkManager.LocalClientId)
            SceneController.LoadNetworkScene(SceneType.Gameplay);
            SetStatus($"Loading scene..");
        }

        private void OnClientDisconnected(ulong clientId)
        {
            SetStatus($"Disconnected: {clientId}");
        }

        private void SetStatus(string s)
        {
            Debug.Log($"[MenuPresenter] {s}");
            _statusText.text = s;
        }
    }
}