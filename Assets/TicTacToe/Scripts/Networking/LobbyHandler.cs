using System;
using Cysharp.Threading.Tasks;
using UniRx;
using Unity.Services.Lobbies.Models;

namespace TicTacToe.Networking
{
    public interface ILobbyHandler
    {
        ReactiveProperty<Lobby> Current { get; }
        UniTask<Lobby> CreateAsync(string name, int maxPlayers, string relayJoinCode);
        UniTask<Lobby> QuickJoinAsync();
        UniTaskVoid UpdateRelayCodeAsync(string code);
        UniTaskVoid DeleteAsync();
    }
    
    public class LobbyHandler : ILobbyHandler, IDisposable
    {
        public ReactiveProperty<Lobby> Current { get; } = new(null);
        CompositeDisposable _disp = new();

        public async UniTask<Lobby> CreateAsync(string name, int maxPlayers, string relayJoinCode)
        {
            Current.Value = await LobbyHelper.CreateLobbyAsync(name, maxPlayers, relayJoinCode);

            return Current.Value;
        }

        public async UniTask<Lobby> QuickJoinAsync()
        {
            var lobby = await LobbyHelper.QuickJoinLobby();
            Current.Value = lobby;
            return lobby;
        }

        public async UniTaskVoid UpdateRelayCodeAsync(string code)
        {
            var lobby = Current.Value;
            if (lobby == null) return;
            
            Current.Value = await LobbyHelper.UpdateLobby(code);
        }

        public async UniTaskVoid DeleteAsync()
        {
            if (Current.Value != null)
            {
                await LobbyHelper.DeleteLobby(Current.Value.Id);
            }

            Current.Value = null;
            _disp?.Dispose();
            _disp = new CompositeDisposable();
        }

        public void Dispose() => _disp?.Dispose();
    }
}