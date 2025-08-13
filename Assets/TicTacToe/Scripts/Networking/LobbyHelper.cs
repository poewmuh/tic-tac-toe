using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;

namespace TicTacToe.Networking
{
    public static class LobbyHelper
    {
        public static async UniTask<Lobby> UpdateLobby(string lobbyId)
        {
            var updateOptions = new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>{
                    { "relayCode", new DataObject(DataObject.VisibilityOptions.Public, lobbyId) }
                }
            };
            
            return await LobbyService.Instance.UpdateLobbyAsync(lobbyId, updateOptions);
        }
        
        public static async UniTask DeleteLobby(string lobbyId)
        {
            await LobbyService.Instance.DeleteLobbyAsync(lobbyId);
        }
        
        public static async UniTaskVoid SendHeartbeatPing(string lobbyId)
        {
            await LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
        }

        public static async UniTask<Lobby> CreateLobbyAsync(string name, int maxPlayers, string relayJoinCode)
        {
            var options = new CreateLobbyOptions
            {
                IsPrivate = false,
                Data = new Dictionary<string, DataObject>{
                    { "relayCode", new DataObject(DataObject.VisibilityOptions.Public, relayJoinCode) }
                }
            };
            return await LobbyService.Instance.CreateLobbyAsync(name, maxPlayers, options);
        }

        public static async UniTask<Lobby> QuickJoinLobby()
        {
            return await LobbyService.Instance.QuickJoinLobbyAsync();
        }

        public static async UniTask<Allocation> AllocateRelay(int maxConnections)
        {
            var allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
            return allocation;
        }
        
        public static async UniTask<string> GetRelayJoinCode(Allocation allocation)
        {
            var relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            return relayJoinCode;
        }
        
        public static async UniTask<JoinAllocation> JoinRelay(string relayJoinCode)
        {
            var joinAllocation = await RelayService.Instance.JoinAllocationAsync(relayJoinCode);
            return joinAllocation;
        }
    }
}