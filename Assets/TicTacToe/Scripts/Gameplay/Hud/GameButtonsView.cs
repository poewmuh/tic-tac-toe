using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace TicTacToe.Gameplay.HUD
{
    public class GameButtonsView : MonoBehaviour
    {
        [SerializeField] private Button _readyButton;
        [SerializeField] private Button _restartButton;
        
        public IObservable<Unit> OnReadyClicked => _readyButton.OnClickAsObservable();
        public IObservable<Unit> OnRestartClicked => _restartButton.OnClickAsObservable();
        
        public void ToggleReady(bool show) => _readyButton.gameObject.SetActive(show);
        public void ToggleRestart(bool show) => _restartButton.gameObject.SetActive(show);
    }
}