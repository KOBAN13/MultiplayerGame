using Db.Interface;
using UnityEngine;

namespace Db.Players
{
    [CreateAssetMenu(fileName = "RemotePlayerParameters", menuName = "Db/RemotePlayerParameters")]
    public class RemotePlayerParameters : ScriptableObject, IRemotePlayerParameters
    {
        [field: SerializeField] public float VisualPositionSmoothTime { get; private set; }
        [field: SerializeField] public float VisualRotationLerpSpeed { get; private set; }
        [field: SerializeField] public float VisualSnapDistance { get; private set; }
        [field: SerializeField] public float RotationSmoothTime { get; private set; }
        [field: SerializeField] public float RotateSpeed { get; private set; }
        [field: SerializeField] public float AngleOverride { get; private set; }
    }
}