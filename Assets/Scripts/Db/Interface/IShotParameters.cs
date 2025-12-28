using UnityEngine;

namespace Db.Interface
{
    public interface IShotParameters
    {
        float DistanceToShot { get;  }
        LayerMask LayerMask { get; }
    }
}