using Unity.Cinemachine.TargetTracking;
using UnityEngine;

namespace Db.Interface
{
    public interface ICameraParameters
    {
        int CameraFOV { get; }
        Vector3 Damping { get; }
        Vector3 ShoulderOffset { get; }
        float CameraSide { get; }
        float CameraDistance { get; }
    }
}