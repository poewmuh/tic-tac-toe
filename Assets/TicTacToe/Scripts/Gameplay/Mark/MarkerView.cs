using System;
using UnityEngine;
using UnityEngine.UI;

namespace TicTacToe.Gameplay.Mark
{
    public class MarkerView : MonoBehaviour
    {
        [SerializeField] private Image _image;
        
        private Color _defaultColor;

        private void Awake()
        {
            _defaultColor = _image.color;
        }

        public void Show()
        {
            _image.enabled = true;
        }

        public void Hide()
        {
            _image.enabled = false;
        }

        public void Highlight()
        {
            _image.color = Color.yellow;
        }
        
        public void StopHighlight()
        {
            _image.color = _defaultColor;
        }
    }
}

