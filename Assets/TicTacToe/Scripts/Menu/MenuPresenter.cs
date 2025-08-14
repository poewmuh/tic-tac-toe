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
        
        private NetworkManager _network;
        private ITransportHandler _transport;
        private IRelayHandler _relay;
        private ILobbyHandler _lobby;
        private readonly CompositeDisposable _disp = new();
        
        [Inject]
        public void Construct(NetworkManager network, ITransportHandler transport, IRelayHandler relay, ILobbyHandler lobby)
        {
            _network = network;
            _transport = transport;
            _relay = relay;
            _lobby = lobby;
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

            _network.OnClientConnectedCallback += OnClientConnected;
            _network.OnClientDisconnectCallback += OnClientDisconnected;
        }

        private void UnSubscribe()
        {
            _network.OnClientConnectedCallback -= OnClientConnected;
            _network.OnClientDisconnectCallback -= OnClientDisconnected;
        }
        
        private async UniTaskVoid StartHost()
        {
            try
            {
                SetStatus("Allocating Relay..");
                var (server, code) = await _relay.AllocateAsync(1);
                _transport.Configure(server);

                SetStatus($"Creating Lobby..");
                await _lobby.CreateAsync("TicTacToeGame", 2, code);

                SetStatus("Starting Host..");
                if (_network.StartHost())
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
                var lobby = await _lobby.QuickJoinAsync();
                var code = lobby.Data["relayCode"].Value;

                SetStatus($"Joining Relay.. code={code}");
                var server = await _relay.JoinAsync(code);
                _transport.Configure(server);

                SetStatus("Starting Client..");
                if (_network.StartClient())
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
            SceneController.LoadNetworkScene(SceneType.Gameplay);
            SetStatus($"Connected: {clientId}");
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