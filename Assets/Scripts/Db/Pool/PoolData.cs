using System.Collections.Generic;
using Db.Dictionary;
using Db.Interface;
using UnityEngine;
using UnityEngine.Serialization;
using Utils.Enums;

namespace Db.Pool
{
    [CreateAssetMenu(fileName = "PoolData", menuName = "Db/PoolData")]
    public class PoolData : ScriptableObject, IPoolData
    {
        [SerializeField] private ObjectPoolsDictionary objectPoolsDictionary;
        
        public IReadOnlyDictionary<EObjectInPoolName, PoolEntry> ObjectPoolsDictionary => objectPoolsDictionary;
    }
}