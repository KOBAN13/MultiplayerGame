using Db.Interface;
using UnityEngine;
using UnityEngine.Serialization;

namespace Db.Players
{
    [CreateAssetMenu(fileName = "LocalPlayerParameters", menuName = "Db/LocalPlayerParameters")]
    public class LocalPlayerParameters : ScriptableObject, ILocalPlayerParameters
    {
        [field: SerializeField] public float RotationSmoothTime { get; private set; }
        
        [field: SerializeField] public float JumpVelocity { get; private set; } = 8f;
        [field: SerializeField] public float Gravity { get; private set; } = -9.81f;
        [field: SerializeField] public float SpeedWalk { get; private set; }
        [field: SerializeField] public float SpeedRun { get; private set; }
    }
}