using Db.Interface;
using Unity.Cinemachine.TargetTracking;
using UnityEngine;

namespace Db
{
    [CreateAssetMenu(fileName = "CameraParameters", menuName = "Db/CameraParameters")]
    public class CameraParameters : ScriptableObject, ICameraParameters
    {
        [field: SerializeField] public int CameraFOV { get; private set; }
        [field: SerializeField] public BindingMode BindingMode { get; private set; }
        [field: SerializeField] public Vector3 PositionDamping { get; private set; }
        [field: SerializeField] public Vector3 FollowOffset { get; private set; }
    }
}