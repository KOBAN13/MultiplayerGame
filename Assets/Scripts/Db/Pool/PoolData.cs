using System.Collections.Generic;
using Db.Dictionary;
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
        [OdinSerialize] private ObjectPoolsDictionary objectPoolsDictionary;
        
        public IReadOnlyDictionary<EObjectInPoolName, PoolEntry> ObjectPoolsDictionary => objectPoolsDictionary;
    }
}