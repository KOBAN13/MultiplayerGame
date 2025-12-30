using UnityEngine;

namespace Utils.Pool
{
    public interface IObjectPool<T> where T : Component
    {
        T Get();
        void ReturnToPool(T obj);
        void Prewarm(int count);
    }
}