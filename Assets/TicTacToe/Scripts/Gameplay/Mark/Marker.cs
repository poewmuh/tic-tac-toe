using TicTacToe.Gameplay.Core;
using UnityEngine;

namespace TicTacToe.Gameplay.Mark
{
    public class Marker : MonoBehaviour
    {
        [SerializeField] private Cell _cell;
        [SerializeField] private MarkerView _markerView;
        
        public Cell Cell => _cell;

        public void StopHighlight()
        {
            _markerView.StopHighlight();
        }

        public void Highlight()
        {
            _markerView.Highlight();
        }
    }
}