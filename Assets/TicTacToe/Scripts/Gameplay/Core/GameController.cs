using System.Linq;
using TicTacToe.Gameplay.Helper;
using UniRx;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace TicTacToe.Gameplay.Core
{
    public class GameController : NetworkBehaviour
    {
        public readonly NetworkVariable<ulong> xClientId = new();
        public readonly NetworkVariable<ulong> oClientId = new();
        public readonly NetworkVariable<ulong> currentTurnClientId = new();
        public readonly NetworkVariable<GameOverInfo> gameOverInfo = new();
        
        public readonly NetworkList<CellValue> board = new();
        private GameSession _gameSession;
        private readonly CompositeDisposable _disp = new();
        
        [Inject]
        public void Construct(GameSession gameSession)
        {
            _gameSession = gameSession;
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                InitializeBoard();
                currentTurnClientId.Value = int.MaxValue;
                _gameSession.OnClientDisconnect().Subscribe(OnClientDisconnect).AddTo(_disp);
                _gameSession.currentState.ObserveValue().Subscribe(StateChanged).AddTo(_disp);
            }
        }

        public override void OnNetworkDespawn()
        {
            _disp.Dispose();
            base.OnNetworkDespawn();
        }

        private void InitializeBoard()
        {
            for (int i = 0; i < 9; i++)
            {
                board.Add(new CellValue(Cell.Empty));
            }
        }

        private void StateChanged(GameState state)
        {
            switch (state)
            {
                case GameState.Playing:
                    ResetBoard();
                    SetRoles();
                    break;
                case GameState.GameOver:
                    break;
            }
        }

        private void OnClientDisconnect(ulong clientId)
        {
            gameOverInfo.Value = new GameOverInfo()
            {
                reason = GameOverReason.Abort,
                winner = Cell.Empty
            };
            SetGameEnd();
        }

        private void SetRoles()
        {
            var isServerStarts = Random.value < .5f;
            var serverId = NetworkManager.ServerClientId;
            var clientId = NetworkManager.ConnectedClientsIds.First(x => x != serverId);
            xClientId.Value = isServerStarts ? serverId : clientId;
            oClientId.Value = isServerStarts ? clientId : serverId;
            currentTurnClientId.Value = xClientId.Value;
        }

        private void SetNextPlayerTurn()
        {
            if (currentTurnClientId.Value == xClientId.Value)
            {
                currentTurnClientId.Value = oClientId.Value;
            }
            else
            {
                currentTurnClientId.Value = xClientId.Value;
            }
        }

        [Rpc(SendTo.Server)]
        public void PlaceMarkRpc(int index, ulong clientId)
        {
            if (_gameSession.currentState.Value is not GameState.Playing) return;
            if (board[index].Value is not Cell.Empty) return;
            
            board[index] = new CellValue(GetCellByClientId(clientId));
            if (TicTacHelper.IsHaveWinner(board, out var winLine))
            {
                gameOverInfo.Value = new GameOverInfo()
                {
                    reason = GameOverReason.Win,
                    winner = clientId == xClientId.Value ? Cell.X : Cell.O,
                    i0 = winLine[0],
                    i1 = winLine[1],
                    i2 = winLine[2],
                };
                SetGameEnd();
            }
            else if (TicTacHelper.IsBoardFull(board))
            {
                gameOverInfo.Value = new GameOverInfo()
                {
                    reason = GameOverReason.Draw,
                    winner = Cell.Empty
                };
                SetGameEnd();
            }
            else
            {
                SetNextPlayerTurn();
            }
        }

        private void SetGameEnd()
        {
            _gameSession.GameEnd();
        }

        private Cell GetCellByClientId(ulong clientId)
        {
            if (clientId == xClientId.Value) return Cell.X;
            return Cell.O;
        }

        private void ResetBoard()
        {
            for (int i = 0; i < board.Count; i++)
            {
                var newCell = board[i];
                newCell.Value = Cell.Empty;
                board[i] = newCell;
            }
        }
    }
}