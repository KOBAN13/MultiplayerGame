using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Utils.Pool
{
    public class ObjectPool<T> : IObjectPool<T> where T : Component
    {
        private readonly Stack<T> _pool = new();
        private readonly GameObject _poolRoot;
        private readonly T _prefab;
        private readonly IObjectResolver _resolver;

        public ObjectPool(GameObject rootObject, T prefab, IObjectResolver resolver)
        {
            _poolRoot = rootObject;
            _prefab = prefab;
            _resolver = resolver;
        }

        public void Prewarm(int count)
        {
            for (var i = 0; i < count; i++)
            {
                var obj = CreateNew();
                obj.gameObject.SetActive(false);
                _pool.Push(obj);
            }
        }

        public async UniTask PrewarmAsync(int count, int batchSize = 10,
            CancellationToken token = default)
        {
            var spawned = 0;

            while (spawned < count)
            {
                if (token.IsCancellationRequested) return;

                var toSpawn = Mathf.Min(batchSize, count - spawned);

                for (var i = 0; i < toSpawn; i++)
                {
                    var obj = CreateNew();
                    obj.transform.SetParent(_poolRoot.transform);
                    obj.gameObject.SetActive(false);
                    _pool.Push(obj);
                    spawned++;
                }

                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
        }

        public T Get()
        {
            while (_pool.Count > 0)
            {
                var pooled = _pool.Pop();
                
                pooled.transform.SetParent(null);
                pooled.gameObject.SetActive(false);
                return pooled;
            }

            var obj = CreateNew();
            obj.transform.SetParent(null);
            obj.gameObject.SetActive(false);
            return obj;
        }

        public void ReturnToPool(T obj)
        {
            if (obj == null)
                return;

            obj.gameObject.SetActive(false);
            obj.transform.SetParent(_poolRoot.transform);
            obj.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            _pool.Push(obj);
        }

        private T CreateNew() => _resolver.Instantiate(_prefab);
    }
}
