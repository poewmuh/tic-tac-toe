using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace TicTacToe.Gameplay.HUD
{
    public class BoardInputView : MonoBehaviour
    {
        [SerializeField] private Button[] _cellButtons;
        
        public IObservable<int> OnCellClicked => _clicks;
        
        private readonly Subject<int> _clicks = new();
        
        private void Awake()
        {
            for (int i = 0; i < _cellButtons.Length; i++)
            {
                int index = i;
                _cellButtons[i].OnClickAsObservable()
                    .Subscribe(_ => _clicks.OnNext(index))
                    .AddTo(this);
            }
        }

        public void SetInteractable(bool interactable)
        {
            foreach (var b in _cellButtons)
                b.interactable = interactable;
        }
    }
}