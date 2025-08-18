using TicTacToe.Gameplay.Core;
using TicTacToe.Gameplay.HUD;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace TicTacToe.Scripts
{
    public class GameplayInstaller : MonoInstaller
    {
        [SerializeField] private GameSession _gameSession;
        [SerializeField] private GameController _gameController;
        
        public override void InstallBindings()
        {
            Container.Bind<GameHUDModel>().AsSingle();
            Container.Bind<GameSession>().FromInstance(_gameSession).AsSingle();
            Container.Bind<GameController>().FromInstance(_gameController).AsSingle();
            Container.Bind<NetworkManager>().FromInstance(NetworkManager.Singleton).AsSingle();
        }
    }
}