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
        private bool _isCrossTurn;
        
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
            
            _gameSession.currentState.ObserveValue().Subscribe(StateChanged).AddTo(this);
            _gameController.currentTurnClientId.ObserveValue()
                .Subscribe(OnTurnChanged).AddTo(this);
        }

        private void StateChanged(GameState state)
        {
            if (state is GameState.GameOver)
            {
                ChangeStatusText("Game Over" + (_isCrossTurn ? " X" : " O") + " Win!!");
                _restartButton.gameObject.SetActive(true);
            }
        }

        private void OnTurnChanged(ulong clientId)
        {
            _isCrossTurn = clientId == _gameController.xClientId.Value;
            ChangeStatusText(clientId == _networkManager.LocalClientId ? "YOUR TURN" : "ENEMY TURN");
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
            _gameSession.SetRestartRpc(_networkManager.LocalClientId);
        }
    }
}