using UnityEngine;
using UnityEngine.UI;

namespace TicTacToe.Gameplay.Mark
{
    public class MarkerView : MonoBehaviour
    {
        [SerializeField] private Image _image;

        public void Show()
        {
            _image.enabled = true;
        }

        public void Hide()
        {
            _image.enabled = false;
        }
    }
}

