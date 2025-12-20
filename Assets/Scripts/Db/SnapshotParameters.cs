using Db.Interface;
using UnityEngine;

namespace Db
{
    [CreateAssetMenu(fileName = "SnapshotParameters", menuName = "Db/SnapshotParameters")]
    public class SnapshotParameters : ScriptableObject, ISnapshotParameters
    {
        [field: SerializeField] public int MaxBufferSize { get; private set; }
        [field: SerializeField] public float InterpolationBackTime { get; private set; }
    }
}