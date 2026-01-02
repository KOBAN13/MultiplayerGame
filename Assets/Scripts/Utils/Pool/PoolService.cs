using System;
using System.Collections.Generic;
using System.Linq;
using Db.Interface;
using UnityEngine;
using Utils.Enums;
using VContainer;

namespace Utils.Pool
{
    public class PoolService : IPoolService
    {
        private readonly Dictionary<EObjectInPoolName, IObjectPool<Component>> _pools = new();
        private readonly Dictionary<EObjectInPoolName, GameObject> _poolRoots = new();
        private IPoolData _poolData;
        private IObjectResolver _objectResolver;

        [Inject]
        private void Construct(IPoolData poolData, IObjectResolver objectResolver)
        {
            _poolData = poolData;
            _objectResolver = objectResolver;
            RegisterPoolRoots();
            RegisterPool();
        }

        public bool TrySpawn<T>(EObjectInPoolName id, bool isActive, out T obj) where T : Component
        {
            obj = Spawn<T>(id);

            if (obj != null)
                obj.gameObject.SetActive(isActive);

            return obj != null;
        }

        public bool TrySpawn<T>(EObjectInPoolName id, bool isActive, Vector3 position, out T obj) where T : Component
        {
            obj = Spawn<T>(id, position);

            if (obj != null)
                obj.gameObject.SetActive(isActive);

            return obj != null;
        }

        public void ReturnToPool<T>(EObjectInPoolName id, T obj) where T : Component
        {
            Debug.LogError("Returning to the pool");
            
            var pool = GetPool<T>(id);

            pool?.ReturnToPool(obj);
        }

        private IObjectPool<Component> GetPool<T>(EObjectInPoolName id) where T : Component
        {
            return _pools.GetValueOrDefault(id);
        }

        private T Spawn<T>(EObjectInPoolName id) where T : Component
        {
            var pool = GetPool<T>(id);

            return pool?.Get() as T;
        }

        private T Spawn<T>(EObjectInPoolName id, Vector3 position) where T : Component
        {
            var pool = GetPool<T>(id);

            if (pool == null)
                return null;

            var obj = pool.Get();
            obj.transform.position = position;
            return obj as T;
        }

        private void RegisterPoolRoots()
        {
            var poolRoots = Enum.GetValues(typeof(EObjectInPoolName)).OfType<EObjectInPoolName>();

            foreach (var rootObject in poolRoots)
            {
                if (rootObject == EObjectInPoolName.None)
                    continue;

                _poolRoots.Add(rootObject, new GameObject(rootObject.ToString()));
            }
        }

        private async void RegisterPool()
        {
            foreach (var entry in _poolData.ObjectPoolsDictionary)
            {
                var typeComponent = entry.Value.objectType;

                if (entry.Value.prefab == null)
                    continue;

                if (!entry.Value.prefab.TryGetComponent(typeComponent, out var component))
                    continue;

                if (entry.Key == EObjectInPoolName.None)
                {
                    var pool = new ObjectPool<Component>(null, component, _objectResolver);
                    await pool.PrewarmAsync(entry.Value.prewarmCount);

                    _pools.Add(entry.Key, pool);
                }
                else
                {
                    if (!_poolRoots.TryGetValue(entry.Key, out var rootObject)) 
                        continue;

                    var pool = new ObjectPool<Component>(rootObject, component, _objectResolver);
                    await pool.PrewarmAsync(entry.Value.prewarmCount);

                    _pools.Add(entry.Key, pool);
                }
            }
        }
    }
}