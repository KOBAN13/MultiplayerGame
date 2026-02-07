using Db.Interface;
using UnityEngine;

namespace Db
{
    [CreateAssetMenu(fileName = "SnapshotParameters", menuName = "Db/SnapshotParameters")]
    public class SnapshotParameters : ScriptableObject, ISnapshotParameters
    {
        [field: SerializeField] public int MaxBufferSize { get; private set; }
        [field: SerializeField] public float InterpolationBackTime { get; private set; }
        [field: SerializeField] public bool UseAdaptiveBackTime { get; private set; }
        [field: SerializeField] public float AdaptiveBackTimeMin { get; private set; }
        [field: SerializeField] public float AdaptiveBackTimeMax { get; private set; }
        [field: SerializeField] public float JitterMultiplier { get; private set; }
        [field: SerializeField] public float JitterSmoothing { get; private set; }
        [field: SerializeField] public float DebugLogInterval { get; private set; }
        [field: SerializeField] public bool EnableInterpolationDebug { get; private set; }
    }
}
