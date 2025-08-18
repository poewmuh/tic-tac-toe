using System.Collections.Generic;
using AiaalTools.Data.Loader;
using TicTacToe.Gameplay.Core;
using TicTacToe.Gameplay.Mark;
using UnityEngine;

namespace TicTacToe.Gameplay.View
{
    public class MarkPool
    {
        private readonly AssetLoaderHandler _crossLoaderHandler;
        private readonly AssetLoaderHandler _circleLoaderHandler;

        private readonly GameObject _crossPrefab;
        private readonly GameObject _circlePrefab;
        
        private readonly Queue<Marker> _circlePool = new();
        private readonly Queue<Marker> _crossPool = new();

        private readonly Transform _parent;
        
        public MarkPool(Transform parent, int startCount)
        {
            _crossLoaderHandler =  new AssetLoaderHandler();
            _circleLoaderHandler = new AssetLoaderHandler(); //надо переделать loadHandler для мультизагрузки
            _crossPrefab = _crossLoaderHandler.LoadImmediate<GameObject>("Cross");
            _circlePrefab = _circleLoaderHandler.LoadImmediate<GameObject>("Circle");
            _parent = parent;

            CreatePool(_crossPrefab, _crossPool, startCount);
            CreatePool(_circlePrefab, _circlePool, startCount);
        }

        public void Dispose()
        {
            _crossLoaderHandler.Unload();
            _circleLoaderHandler.Unload();
            _circlePool.Clear();
            _crossPool.Clear();
        }

        private void CreatePool(GameObject prefab, Queue<Marker> pool, int count)
        {
            for (int i = 0; i < count; i++)
            {
                var marker = CreateMarker(prefab);
                marker.gameObject.SetActive(false);
                pool.Enqueue(marker);
            }
        }
        
        public Marker GetFromPool(Cell cellType, Vector3 localPos)
        {
            var isCross = cellType is Cell.X;
            var pool = isCross ? _crossPool : _circlePool;
            var prefab = isCross ? _crossPrefab : _circlePrefab;
            
            var marker = GetFromPool(prefab, pool);
            marker.transform.localPosition = localPos;
            marker.gameObject.SetActive(true);
            return marker;
        }

        public void Return(Marker marker)
        {
            marker.gameObject.SetActive(false);
            marker.transform.SetParent(_parent);
            if (marker.Cell is Cell.O)
                _circlePool.Enqueue(marker);
            else
                _crossPool.Enqueue(marker);
        }

        private Marker GetFromPool(GameObject prefab, Queue<Marker> pool)
        {
            if (pool.Count > 0)
                return pool.Dequeue();

            var marker = CreateMarker(prefab);
            marker.gameObject.SetActive(false);
            return marker;
        }

        private Marker CreateMarker(GameObject prefab)
        {
            return Object.Instantiate(prefab, _parent).GetComponent<Marker>();
        }
    }
}