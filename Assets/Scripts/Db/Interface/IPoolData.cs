using System.Collections.Generic;
using Db.Pool;
using Utils.Enums;

namespace Db.Interface
{
    public interface IPoolData
    {
        IReadOnlyDictionary<EObjectInPoolName, PoolEntry> ObjectPoolsDictionary { get; }
    }
}