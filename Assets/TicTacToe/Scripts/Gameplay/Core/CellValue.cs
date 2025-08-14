using Unity.Netcode;

namespace TicTacToe.Gameplay.Core
{
    public enum Cell
    {
        Empty,
        X,
        O
    }

    public struct CellValue : INetworkSerializable, System.IEquatable<CellValue>
    {
        public Cell Value;

        public CellValue(Cell value)
        {
            Value = value;
        }
        
        public bool Equals(CellValue other) => Value == other.Value;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Value);
        }
    }
}