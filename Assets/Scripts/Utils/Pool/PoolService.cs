using System;
using System.Collections.Generic;
using System.Linq;
using Db.Interface;
using UnityEngine;
using VContainer;

namespace Utils.Pool
{
 public class PoolService : IPoolService
    {
        private readonly Dictionary<string, IObjectPool<Component>> _pools = new();
        private readonly Dictionary<EPoolRootObject, GameObject> _poolRoots = new();
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
        
        public bool TrySpawn<T>(string id, bool isActive, out T obj) where T : Component
        {
            obj = Spawn<T>(id);
            
            if (obj != null)
                obj.gameObject.SetActive(isActive);
            
            return obj != null;
        }
        
        public bool TrySpawn<T>(string id, bool isActive, Vector3 position, out T obj) where T : Component
        {
            obj = Spawn<T>(id, position);
            
            if (obj != null)
                obj.gameObject.SetActive(isActive);
            
            return obj != null;
        }
        
        public void ReturnToPool<T>(string id, T obj) where T : Component
        {
            var pool = GetPool<T>(id);

            pool?.ReturnToPool(obj);
        }

        private IObjectPool<Component> GetPool<T>(string id) where T : Component
        {
            return _pools.GetValueOrDefault(id);
        }
        
        private T Spawn<T>(string id) where T : Component
        {
            var pool = GetPool<T>(id);
            
            return pool?.Get() as T;
        }

        private T Spawn<T>(string id, Vector3 position) where T : Component
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
            var poolRoots = Enum.GetValues(typeof(EPoolRootObject)).OfType<EPoolRootObject>();
            
            foreach (var rootObject in poolRoots)
            {
                if (rootObject == EPoolRootObject.None) continue;
                _poolRoots.Add(rootObject, new GameObject(rootObject.ToString()));
            }
        }
        
        private async void RegisterPool()
        {
            foreach (var entry in _poolData.ObjectPools)
            {
                var typeComponent = entry.Value.objectType;
                
                if(entry.Value.prefab == null) continue;
                if (!entry.Value.prefab.TryGetComponent(typeComponent, out var component)) continue;

                if (entry.Value.rootObject == EPoolRootObject.None)
                {
                    var pool = new ObjectPool<Component>(null, component, _objectResolver);
                    await pool.PrewarmAsync(entry.Value.prewarmCount);
                
                    _pools.Add(entry.Key, pool);
                }
                else
                {
                    if (!_poolRoots.TryGetValue(entry.Value.rootObject, out var rootObject)) continue;
                    
                    var pool = new ObjectPool<Component>(rootObject, component, _objectResolver);
                    await pool.PrewarmAsync(entry.Value.prewarmCount);
                
                    _pools.Add(entry.Key, pool);
                }
            }
        }
    }
}
}