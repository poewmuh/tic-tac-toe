using UniRx;
using UnityEngine;
using Zenject;

namespace TicTacToe.Gameplay.HUD
{
    public class GameHUD : MonoBehaviour
    {
        [SerializeField] private GameButtonsView _buttonsView;
        [SerializeField] private StatusBannerView _statusBannerView;
        [SerializeField] private BoardInputView _boardInputView;

        [Inject] private GameHUDModel _gameHudModel;

        private void Start()
        {
            _buttonsView.OnReadyClicked
                .Subscribe(_ => _gameHudModel.readyCommand.Execute())
                .AddTo(this);

            _buttonsView.OnRestartClicked
                .Subscribe(_ => _gameHudModel.restartCommand.Execute())
                .AddTo(this);

            _boardInputView.OnCellClicked
                .Subscribe(i => _gameHudModel.placeMarkCommand.Execute(i))
                .AddTo(this);

            _gameHudModel.statusText.Subscribe(t => _statusBannerView.SetStatus(t)).AddTo(this);
            _gameHudModel.readyVisible.Subscribe(v => _buttonsView.ToggleReady(v)).AddTo(this);
            _gameHudModel.restartVisible.Subscribe(v => _buttonsView.ToggleRestart(v)).AddTo(this);
            _gameHudModel.boardInputInteractable.Subscribe(v => _boardInputView.SetInteractable(v)).AddTo(this);
        }

        private void OnDestroy()
        {
            _gameHudModel.Dispose();
        }
    }
}