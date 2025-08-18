using System.Collections.Generic;
using TicTacToe.Gameplay.Core;
using TicTacToe.Gameplay.Helper;
using TicTacToe.Gameplay.Mark;
using UniRx;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace TicTacToe.Gameplay.View
{
    public class BoardDrawer : MonoBehaviour
    {
        [SerializeField] private Transform _drawAreaParent;
        [SerializeField] private float _cellSizeX;
        [SerializeField] private float _cellSizeY;

        
        private readonly Dictionary<int, Marker> _activeMarkers = new ();
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

            _markPool = new MarkPool(_drawAreaParent, 3);
        }

        private void OnDestroy()
        {
            _activeMarkers.Clear();
            _markPool.Dispose();
        }

        private void RefreshBoard(NetworkListEvent<CellValue> board)
        {
            var pos = IndexToLocalPos(board.Index);
            if (board.Value.Value == Cell.Empty)
            {
                if (_activeMarkers.TryGetValue(board.Index, out var marker))
                {
                    _markPool.Return(marker);
                    _activeMarkers.Remove(board.Index);
                }
                
                return;
            }
            var mark = _markPool.GetFromPool(board.Value.Value, pos);
            _activeMarkers.Add(board.Index, mark);
        }

        private Vector3 IndexToLocalPos(int index)
        {
            int row = index / 3;
            int col = index % 3;
            Debug.Log($"Index: {index}, Row: {row}, Col: {col}");
            return new Vector3(col * _cellSizeX, row * _cellSizeY, 0f);
        }
    }
}