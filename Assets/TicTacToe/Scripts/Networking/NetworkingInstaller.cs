using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using Zenject;

namespace TicTacToe.Networking
{
    public class NetworkingInstaller : MonoInstaller
    {
        [SerializeField] private NetworkManager _networkManager;
        [SerializeField] private UnityTransport _transport;
        
        public override void InstallBindings()
        {
            Container.Bind<NetworkManager>().FromInstance(_networkManager).AsSingle();
            Container.Bind<UnityTransport>().FromInstance(_transport).AsSingle();
            
            Container.BindInterfacesTo<ServicesInitializer>().AsSingle().NonLazy();

            Container.BindInterfacesAndSelfTo<RelayHandler>().AsSingle();
            Container.BindInterfacesAndSelfTo<LobbyHandler>().AsSingle();
            Container.BindInterfacesAndSelfTo<TransportHandler>().AsSingle();
        }
    }
}