using Unity.Netcode;
using UnityEngine;

namespace TicTacToe.Gameplay.Core
{
    public class GameSession : NetworkBehaviour
    {
        public readonly NetworkVariable<GameState> currentState = new();

        private int _maxPlayers = 2;
        private int _readyCount;

        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;
            SetStateMaster(GameState.WaitingPlayers);
        }

        [Rpc(SendTo.Server)]
        public void SetReadyRpc(ulong senderId)
        {
            Debug.Log(senderId + " is ready");
            _readyCount++;
            if (_readyCount == _maxPlayers)
            {
                SetStateMaster(GameState.Playing);
                _readyCount = 0;
            }
        }
        
        [Rpc(SendTo.Server)]
        public void SetRestartRpc(ulong senderId)
        {
            Debug.Log(senderId + " is ready");
            _readyCount++;
            if (_readyCount == _maxPlayers)
            {
                SetStateMaster(GameState.Playing);
                _readyCount = 0;
            }
        }
        
        [Rpc(SendTo.Server)]
        public void PlaceMarkRpc(ulong senderId)
        {
            Debug.Log(senderId + " placed mark");
        }

        private void SetStateMaster(GameState newState)
        {
            currentState.Value = newState;
        }
    }
}