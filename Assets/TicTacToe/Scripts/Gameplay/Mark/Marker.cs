using TicTacToe.Gameplay.Core;
using UnityEngine;

namespace TicTacToe.Gameplay.Mark
{
    public class Marker : MonoBehaviour
    {
        [SerializeField] private Cell _cell;
        [SerializeField] private MarkerView markerView;
        
        public Cell Cell => _cell;
    }
}