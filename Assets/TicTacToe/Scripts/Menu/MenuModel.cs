using System;
using Cysharp.Threading.Tasks;
using TicTacToe.Networking;
using UniRx;
using Unity.Netcode;
using UnityEngine;

namespace TicTacToe.Menu
{
    public interface IMenuModel : IDisposable
    {
        ReactiveCommand HostCmd { get; }
        ReactiveCommand QuickJoinCmd { get; }
        IReadOnlyReactiveProperty<string> Status { get; }
    }
    
    public class MenuModel : IMenuModel
    {
        public ReactiveCommand HostCmd { get; } = new();
        public ReactiveCommand QuickJoinCmd { get; } = new();
        public IReadOnlyReactiveProperty<string> Status => _status;
        
        private readonly ReactiveProperty<string> _status = new("Ready.");
        
        private readonly NetworkManager _networkManager;
        private readonly ITransportHandler _unityTransport;
        private readonly IRelayHandler _relaySystem;
        private readonly ILobbyHandler _lobbySystem;
        
        private readonly CompositeDisposable _disp = new();
        
        public MenuModel(NetworkManager network, ITransportHandler transport, IRelayHandler relay, ILobbyHandler lobby)
        {
            _networkManager = network;
            _unityTransport = transport;
            _relaySystem = relay;
            _lobbySystem = lobby;
            
            HostCmd.Subscribe(_ => StartHost().Forget()).AddTo(_disp);
            QuickJoinCmd.Subscribe(_ => StartJoin().Forget()).AddTo(_disp);
        }
        
        public void Dispose() => _disp.Dispose();

        private async UniTaskVoid StartHost()
        {
            try
            {
                UpdateStatus("Allocating Relay..");
                var (server, code) = await _relaySystem.AllocateAsync(1);
                _unityTransport.Configure(server);

                UpdateStatus($"Creating Lobby..");
                await _lobbySystem.CreateAsync("TicTacToeGame", 2, code);

                UpdateStatus("Starting Host..");
                if (_networkManager.StartHost())
                {
                    UpdateStatus($"HOSTED. CODE: {code}");
                }
                else
                {
                    UpdateStatus("StartHost failed.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                UpdateStatus("Error");
            }
        }

        private async UniTaskVoid StartJoin()
        {
            try
            {
                UpdateStatus("Quick Joining Lobby..");
                var lobby = await _lobbySystem.QuickJoinAsync();
                var code = lobby.Data["relayCode"].Value;

                UpdateStatus($"Joining Relay.. code={code}");
                var server = await _relaySystem.JoinAsync(code);
                _unityTransport.Configure(server);

                UpdateStatus("Starting Client..");
                if (_networkManager.StartClient())
                {
                    UpdateStatus("CONNECTED");
                }
                else
                {
                    UpdateStatus("Connection failed");
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                UpdateStatus("Error");
            }
        }

        private void UpdateStatus(string status)
        {
            _status.Value = status;
        }
    }
}