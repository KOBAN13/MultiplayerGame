using System;
using Db.Pool;
using Utils.Enums;
using Utils.SerializedDictionary;

namespace Db.Dictionary
{
    [Serializable]
    public class ObjectPoolsDictionary : 
        UnitySerializedDictionaryBase<EObjectInPoolName, PoolEntry>
    {
        
    }
}