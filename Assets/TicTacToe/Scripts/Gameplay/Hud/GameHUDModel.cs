using System;
using TicTacToe.Gameplay.Core;
using TicTacToe.Gameplay.Helper;
using UniRx;
using Unity.Netcode;
using Zenject;

namespace TicTacToe.Gameplay.HUD
{
    public class GameHUDModel : IDisposable
    {
        public readonly ReactiveProperty<string> statusText = new("Waiting ready…");
        public readonly ReactiveProperty<bool> readyVisible = new(true);
        public readonly ReactiveProperty<bool> restartVisible = new(false);
        public readonly ReactiveProperty<bool> boardInputInteractable = new(false);
        
        public readonly ReactiveCommand readyCommand = new();
        public readonly ReactiveCommand restartCommand = new();
        public readonly ReactiveCommand<int> placeMarkCommand = new();
        
        private readonly NetworkManager _networkManager;
        private readonly GameSession _gameSession;
        private readonly GameController _gameController;
        private readonly CompositeDisposable _disp = new();

        [Inject]
        public GameHUDModel(GameSession gameSession, GameController gameController, NetworkManager networkManager)
        {
            _networkManager = networkManager;
            _gameSession = gameSession;
            _gameController = gameController;

            readyCommand.Subscribe(_ => OnReady()).AddTo(_disp);
            restartCommand.Subscribe(_ => OnRestart()).AddTo(_disp);
            placeMarkCommand.Subscribe(OnPlaceMark).AddTo(_disp);

            _gameSession.currentState.ObserveValue()
                .Subscribe(OnStateChanged)
                .AddTo(_disp);

            _gameController.currentTurnClientId.ObserveValue()
                .Subscribe(OnTurnChanged)
                .AddTo(_disp);

            _gameController.gameOverInfo.ObserveValue()
                .Subscribe(OnGameOver)
                .AddTo(_disp);
        }
        
        private void OnReady()
        {
            readyVisible.Value = false;
            statusText.Value = "Waiting opponent…";
            _gameSession.SetReadyRpc();
        }
        
        private void OnRestart()
        {
            restartVisible.Value = false;
            boardInputInteractable.Value = false;
            statusText.Value = "Waiting opponent…";
            _gameSession.SetRestartRpc();
        }
        
        private void OnPlaceMark(int index)
        {
            _gameController.PlaceMarkRpc(index);
        }
        
        private void OnStateChanged(GameState s)
        {
            switch (s)
            {
                case GameState.Playing:
                {
                    UpdateTurnStatus(_gameController.currentTurnClientId.Value);
                    break;
                }
                case GameState.GameOver:
                {
                    boardInputInteractable.Value = false;
                    break;
                }
            }
        }
        
        private void OnTurnChanged(ulong clientId)
        {
            UpdateTurnStatus(clientId);
        }
        
        private void UpdateTurnStatus(ulong currentTurnClientId)
        {
            var myTurn = currentTurnClientId == _networkManager.LocalClientId;
            statusText.Value = myTurn ? "YOUR TURN" : "ENEMY TURN";
            boardInputInteractable.Value = myTurn;
        }
        
        private void OnGameOver(GameOverInfo info)
        {
            switch (info.reason)
            {
                case GameOverReason.Draw:
                    statusText.Value = "Draw!";
                    restartVisible.Value = true;
                    break;

                case GameOverReason.Win:
                    statusText.Value = (info.winner == Cell.O ? "O" : "X") + " ARE WINNER!";
                    restartVisible.Value = true;
                    break;

                case GameOverReason.Abort:
                    statusText.Value = "GAME ABORTED!";
                    restartVisible.Value = false;
                    break;
            }

            boardInputInteractable.Value = false;
        }
        
        public void Dispose()
        {
            _disp.Dispose();
        }
    }
}