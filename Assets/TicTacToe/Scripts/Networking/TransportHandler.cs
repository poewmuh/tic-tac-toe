using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using UnityEngine;

namespace TicTacToe.Networking
{
    public interface ITransportHandler
    {
        void Configure(RelayServerData data);
    }
    
    public class TransportHandler : ITransportHandler
    {
        private readonly UnityTransport _transport;

        public TransportHandler(UnityTransport transport)
        {
            _transport = transport;
        }

        public void Configure(RelayServerData data)
        {
            _transport.SetRelayServerData(data);
        }
    }
}