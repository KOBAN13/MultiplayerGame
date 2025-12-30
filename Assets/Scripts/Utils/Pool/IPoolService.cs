using UnityEngine;
using Utils.Enums;

namespace Utils.Pool
{
    public interface IPoolService
    {
        bool TrySpawn<T>(EObjectInPoolName id, bool isActive, out T obj) where T : Component;
        bool TrySpawn<T>(EObjectInPoolName id, bool isActive, Vector3 position, out T obj) where T : Component;
        void ReturnToPool<T>(EObjectInPoolName id, T obj) where T : Component;
    }
}