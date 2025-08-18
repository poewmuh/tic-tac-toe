using UnityEngine;
using Zenject;

namespace TicTacToe.Menu
{
    public class MenuInstaller : MonoInstaller
    {
        [SerializeField] private MenuView _menuView;
        
        public override void InstallBindings()
        {
            Container.Bind<MenuView>().FromInstance(_menuView).AsSingle();
            Container.Bind<IMenuModel>().To<MenuModel>().AsSingle();
        }
    }
}