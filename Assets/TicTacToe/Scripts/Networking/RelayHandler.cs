using Cysharp.Threading.Tasks;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay.Models;

namespace TicTacToe.Networking
{
    public interface IRelayHandler
    {
        UniTask<(RelayServerData serverData, string joinCode)> AllocateAsync(int maxConnections);
        UniTask<RelayServerData> JoinAsync(string joinCode);
    }
    
    public class RelayHandler : IRelayHandler
    {
        public async UniTask<(RelayServerData, string)> AllocateAsync(int maxConnections)
        {
            var alloc = await LobbyHelper.AllocateRelay(maxConnections);
            var code = await LobbyHelper.GetRelayJoinCode(alloc);
            var serverData = alloc.ToRelayServerData("dtls");
            return (serverData, code);
        }

        public async UniTask<RelayServerData> JoinAsync(string joinCode)
        {
            var joinAlloc = await LobbyHelper.JoinRelay(joinCode);
            return joinAlloc.ToRelayServerData("dtls");
        }
    }
}