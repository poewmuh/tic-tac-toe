using TMPro;
using UnityEngine;

namespace TicTacToe.Gameplay.HUD
{
    public class StatusBannerView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _statusText;
        
        public void SetStatus(string status) => _statusText.text = status;
    }
}