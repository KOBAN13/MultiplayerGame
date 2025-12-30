using System.Collections.Generic;
using Db.Interface;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using Utils.Enums;

namespace Db.Pool
{
    [CreateAssetMenu(fileName = "PoolData", menuName = "Db/PoolData")]
    public class PoolData : SerializedScriptableObject, IPoolData
    {
        [OdinSerialize] private Dictionary<EObjectInPoolName, PoolEntry> _objectPoolsDictionary;
        
        public IReadOnlyDictionary<EObjectInPoolName, PoolEntry> ObjectPoolsDictionary => _objectPoolsDictionary;
    }
}