using TicTacToe.Gameplay.Core;
using TicTacToe.Gameplay.Helper;
using TMPro;
using UniRx;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TicTacToe.Gameplay.HUD
{
    public class GameHUD : MonoBehaviour
    {
        [SerializeField] private Button _readyButton;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button[] _cellButton;
        [SerializeField] private TextMeshProUGUI _statusText;

        private GameSession _gameSession;
        private GameController _gameController;
        private NetworkManager _networkManager;
        
        [Inject]
        private void Construct(GameSession gameSession, GameController gameController, NetworkManager networkManager)
        {
            _gameSession = gameSession;
            _gameController = gameController;
            _networkManager = networkManager;
        }

        private void Start()
        {
            ChangeStatusText("Waiting ready..");
            
            _readyButton.OnClickAsObservable()
                .Subscribe(_ => SendReady()).AddTo(this);
            _restartButton.OnClickAsObservable()
                .Subscribe(_ => SendRestart()).AddTo(this);

            for (int i = 0; i < _cellButton.Length; i++)
            {
                var index = i;
                _cellButton[i].OnClickAsObservable()
                    .Subscribe(_ => TrySendMark(index)).AddTo(this);
            }

            _gameSession.currentState.ObserveValue().Subscribe(OnStateChange).AddTo(this);
            _gameController.gameOverInfo.ObserveValue().Subscribe(OnGameOver).AddTo(this);
            _gameController.currentTurnClientId.ObserveValue()
                .Subscribe(OnTurnChanged).AddTo(this);
        }

        private void OnGameOver(GameOverInfo gameOverInfo)
        {
            var statusText = "";
            switch (gameOverInfo.reason)
            {
                case GameOverReason.Draw:
                    statusText = "Draw!";
                    break;
                case GameOverReason.Win:
                    statusText = (gameOverInfo.winner is Cell.O ? "O" : "X") + " ARE WINNER!";
                    break;
                case GameOverReason.Abort:
                    statusText = "GAME ABORTED!";
                    break;
            }

            ChangeStatusText(statusText);

            if (gameOverInfo.reason is not GameOverReason.Abort)
            {
                _restartButton.gameObject.SetActive(true);
            }
        }

        private void OnStateChange(GameState newState)
        {
            if (newState is GameState.Playing)
            {
                UpdateTurnStatus(_gameController.currentTurnClientId.Value);
            }
        }

        private void OnTurnChanged(ulong clientId)
        {
            UpdateTurnStatus(clientId);
        }

        private void UpdateTurnStatus(ulong currentTurnClientId)
        {
            ChangeStatusText(currentTurnClientId == _networkManager.LocalClientId ? "YOUR TURN" : "ENEMY TURN");
        }

        private void ChangeStatusText(string text)
        {
            _statusText.text = text;
        }

        private void TrySendMark(int index)
        {
            if (_gameController.currentTurnClientId.Value == _networkManager.LocalClientId)
            {
                _gameController.PlaceMarkRpc(index, _networkManager.LocalClientId);
            }
        }

        private void SendReady()
        {
            _readyButton.gameObject.SetActive(false);
            _gameSession.SetReadyRpc(_networkManager.LocalClientId);
        }

        private void SendRestart()
        {
            _restartButton.gameObject.SetActive(false);
            ChangeStatusText("Waiting your opponent..");
            _gameSession.SetRestartRpc(_networkManager.LocalClientId);
        }
    }
}