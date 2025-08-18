using TicTacToe.Gameplay.Helper;
using TicTacToe.Global;
using UniRx;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace TicTacToe.Menu
{
    public class MenuModelView : MonoBehaviour
    {
        private MenuView _menuView;
        private IMenuModel _menuModel;
        private NetworkManager _networkManager;
        private readonly CompositeDisposable _disp = new();
        
        [Inject]
        public void Construct(NetworkManager network, MenuView view, IMenuModel model)
        {
            _networkManager = network;
            _menuView = view;
            _menuModel = model;
        }
        
        private void Start()
        {
            Subscribe();
        }
        
        private void Subscribe()
        {
            _menuModel.Status.Subscribe(SetStatus).AddTo(_disp);
            _menuView.OnHostClick.Subscribe(_ => _menuModel.HostCmd.Execute()).AddTo(_disp);
            _menuView.OnQuickJoinClick.Subscribe(_ => _menuModel.QuickJoinCmd.Execute()).AddTo(_disp);
            _networkManager.ObserveClientConnected().Subscribe(OnClientConnected).AddTo(_disp);
            _networkManager.ObserveClientDisconnected().Subscribe(OnClientDisconnected).AddTo(_disp);
        }
        
        private void OnClientConnected(ulong clientId)
        {
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
            _menuView.SetStatus(s);
        }

        private void OnDestroy()
        {
            _menuModel.Dispose();
            _disp.Dispose();
        }
    }
}