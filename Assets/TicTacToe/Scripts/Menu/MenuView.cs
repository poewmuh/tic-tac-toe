using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace TicTacToe.Menu
{
    public class MenuView : MonoBehaviour
    {
        [SerializeField] private Button _hostButton;
        [SerializeField] private Button _quickJoinButton;
        [SerializeField] private TextMeshProUGUI statusText;

        public IObservable<Unit> OnHostClick => _hostButton.OnClickAsObservable();
        public IObservable<Unit> OnQuickJoinClick => _quickJoinButton.OnClickAsObservable();

        public void SetStatus(string s)
        {
            if (statusText) statusText.text = s;
            Debug.Log($"[MenuView] {s}");
        }
    }
}