using System.Collections.Generic;
using AiaalTools.Data.Loader;
using UnityEngine;

namespace TicTacToe.Gameplay.View
{
    public class MarkPool
    {
        private readonly AssetLoaderHandler _crossLoaderHandler;
        private readonly AssetLoaderHandler _circleLoaderHandler;

        private readonly GameObject _crossPrefab;
        private readonly GameObject _circlePrefab;
        
        private readonly Queue<GameObject> _circlePool = new();
        private readonly Queue<GameObject> _crossPool = new();

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

        private void CreatePool(GameObject prefab, Queue<GameObject> pool, int count)
        {
            for (int i = 0; i < count; i++)
            {
                var go = Object.Instantiate(prefab, _parent);
                go.SetActive(false);
                pool.Enqueue(go);
            }
        }
        
        public GameObject GetOrCreateCircle(Vector3 pos)
        {
            var go = GetFromPool(_circlePrefab, _circlePool);
            go.transform.position = pos;
            go.SetActive(true);
            return go;
        }

        public GameObject GetOrCreateCross(Vector3 pos)
        {
            var go = GetFromPool(_crossPrefab, _crossPool);
            go.transform.position = pos;
            go.SetActive(true);
            return go;
        }

        public void Return(GameObject go, bool isCircle)
        {
            go.SetActive(false);
            go.transform.SetParent(_parent);
            if (isCircle)
                _circlePool.Enqueue(go);
            else
                _crossPool.Enqueue(go);
        }

        private GameObject GetFromPool(GameObject prefab, Queue<GameObject> pool)
        {
            if (pool.Count > 0)
                return pool.Dequeue();

            var go = Object.Instantiate(prefab, _parent);
            go.SetActive(false);
            return go;
        }
    }
}