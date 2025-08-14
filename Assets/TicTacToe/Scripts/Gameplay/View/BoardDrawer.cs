using System;
using TicTacToe.Gameplay.Core;
using TicTacToe.Gameplay.Helper;
using UniRx;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace TicTacToe.Gameplay.View
{
    public class BoardDrawer : MonoBehaviour
    {
        [SerializeField] private Transform _drawArea;
        [SerializeField] private Transform[] _boardPositions;

        private GameController _gameController;
        private MarkPool _markPool;
        
        [Inject]
        private void Construct(GameController gameController)
        {
            _gameController = gameController;
        }

        private void Start()
        {
            _gameController.board.ObserveListChanged()
                .Subscribe(RefreshBoard).AddTo(this);

            _markPool = new MarkPool(_drawArea, 3);
        }

        private void OnDestroy()
        {
            _markPool.Dispose();
        }

        private void RefreshBoard(NetworkListEvent<CellValue> board)
        {
            var isCircle = board.Value.Value is Cell.O;
            var pos = _boardPositions[board.Index].position;
            if (board.Value.Value == Cell.Empty)
            {
                //clearing
            }
            var mark = isCircle ? _markPool.GetOrCreateCircle(pos) : _markPool.GetOrCreateCross(pos);
        }
        
        
    }
}