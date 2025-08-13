using TicTacToe.Global;
using UnityEngine;
using UnityEngine.Serialization;

namespace TicTacToe.Boot
{
    public class Boot : MonoBehaviour
    {
        [SerializeField] private LogoAnimationView logoAnimationView;

        private void Awake()
        {
            Subscribe();
        }

        private void OnDestroy()
        {
            UnSubscribe();
        }

        private void Subscribe()
        {
            logoAnimationView.OnAnimationEnd += OnAnimationViewEnd;
        }

        private void UnSubscribe()
        {
            logoAnimationView.OnAnimationEnd -= OnAnimationViewEnd;
        }

        private void OnAnimationViewEnd()
        {
            SceneController.LoadSceneLocal(SceneType.Menu);
        }
    }
}