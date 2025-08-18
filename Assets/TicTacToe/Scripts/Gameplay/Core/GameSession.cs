using System;
using TicTacToe.Gameplay.Helper;
using Unity.Netcode;
using UnityEngine;

namespace TicTacToe.Gameplay.Core
{
    public class GameSession : NetworkBehaviour
    {
        private const int MAX_PLAYERS = 2;
        
        public readonly NetworkVariable<GameState> currentState = new();
        public IObservable<ulong> OnClientDisconnect() => NetworkManager.Singleton.ObserveClientDisconnected();
        
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
            if (_readyCount == MAX_PLAYERS)
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
            if (_readyCount == MAX_PLAYERS)
            {
                SetStateMaster(GameState.Playing);
                _readyCount = 0;
            }
        }

        public void GameEnd()
        {
            SetStateMaster(GameState.GameOver);
        }

        private void SetStateMaster(GameState newState)
        {
            currentState.Value = newState;
        }
    }
}