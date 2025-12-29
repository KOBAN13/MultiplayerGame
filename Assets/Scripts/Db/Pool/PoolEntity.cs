using System;
using UnityEngine;

namespace Db.Pool
{
    [Serializable]
    public class PoolEntry
    {
        public GameObject prefab;
        public int prewarmCount = 10;
        public Type objectType;
    }
}