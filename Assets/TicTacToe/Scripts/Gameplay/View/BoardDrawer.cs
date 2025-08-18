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

        private readonly HashSet<int> _pendingHighlights = new();
        private readonly Dictionary<int, Marker> _activeMarkers = new ();
        private GameController _gameController;
        private MarkPool _markPool;

        private bool _isGameOver;
        
        [Inject]
        private void Construct(GameController gameController)
        {
            _gameController = gameController;
        }

        private void Start()
        {
            _gameController.gameOverInfo.ObserveValue().Subscribe(OnGameOver).AddTo(this);
            _gameController.board.ObserveListChanged()
                .Subscribe(RefreshBoard).AddTo(this);

            _markPool = new MarkPool(_drawAreaParent, 3);
        }

        private void OnGameOver(GameOverInfo info)
        {
            _isGameOver = true;
            if (info.reason == GameOverReason.Win)
            {
                TryHighlight(info.i0);
                TryHighlight(info.i1);
                TryHighlight(info.i2);
            }
        }

        private void TryHighlight(int index)
        {
            if (_activeMarkers.TryGetValue(index, out var marker))
            {
                marker.Highlight();
            }
            else
            {
                _pendingHighlights.Add(index);
            }
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
                    _isGameOver = false;
                    _markPool.Return(marker);
                    _activeMarkers.Remove(board.Index);
                }
                
                return;
            }
            var mark = _markPool.GetFromPool(board.Value.Value, pos);
            _activeMarkers[board.Index] = mark;
            if (_isGameOver && _pendingHighlights.Remove(board.Index))
            {
                mark.Highlight();
            }
        }

        private Vector3 IndexToLocalPos(int index)
        {
            int row = index / 3;
            int col = index % 3;
            return new Vector3(col * _cellSizeX, row * _cellSizeY, 0f);
        }
    }
}