using Unity.Cinemachine.TargetTracking;
using UnityEngine;

namespace Db.Interface
{
    public interface ICameraParameters
    {
        int CameraFOV { get; }
        BindingMode BindingMode { get; }
        Vector3 PositionDamping { get; }
        Vector3 FollowOffset { get; }
    }
}