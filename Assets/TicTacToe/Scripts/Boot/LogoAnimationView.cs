using System;
using DG.Tweening;
using UnityEngine;

namespace TicTacToe.Boot
{
    public class LogoAnimationView : MonoBehaviour
    {
        public event Action OnAnimationEnd;
        
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _showHideDuration = 1;
        [SerializeField] private float _pauseDuration = 1;
        
        private Sequence _animationSequence;
        
        private void Start()
        {
            StartAnimation();
        }

        private void StartAnimation()
        {
            _canvasGroup.alpha = 0;
            
            _animationSequence = DOTween.Sequence();
            _animationSequence
                .Append(_canvasGroup.DOFade(1, _showHideDuration))
                .AppendInterval(_pauseDuration)
                .Append(_canvasGroup.DOFade(0, _showHideDuration))
                .OnComplete(() =>
                {
                    DOTween.Kill(_animationSequence);
                    OnAnimationEnd?.Invoke();
                }); 
        }
    }
}