using System;
using Unity.Netcode;
using UnityEngine;

namespace TicTacToe.Gameplay.Core
{
    public enum GameOverReason : byte
    {
        Win = 0,
        Draw = 1,
        Abort = 2
    }
    
    public struct GameOverInfo : INetworkSerializable, IEquatable<GameOverInfo>
    {
        public GameOverReason reason; 
        public Cell winner;
        public int i0, i1, i2;
        public int gameCount;

        public void NetworkSerialize<T>(BufferSerializer<T> s) where T : IReaderWriter
        {
            s.SerializeValue(ref reason);
            s.SerializeValue(ref winner);
            s.SerializeValue(ref i0);
            s.SerializeValue(ref i1);
            s.SerializeValue(ref i2);
            s.SerializeValue(ref gameCount);
        }

        public bool Equals(GameOverInfo other)
        {
            return reason == other.reason && winner == other.winner &&
                   i0 == other.i0 && i1 == other.i1 && i2 == other.i2 &&
                   gameCount == other.gameCount;
        }
    }
}