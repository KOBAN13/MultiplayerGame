using UnityEngine;

namespace Db.Interface
{
    public interface IRotationCameraParameters
    {
        LayerMask AimColliderLayerMask { get; }
        float RaycastDistance { get; }
        float RotateSpeed { get; }
        
        float Sensitivity { get; }
        float TopClamp { get; }
        float BottomClamp { get; }
        float AngleOverride { get; }
    }
}