using Db.Interface;
using UnityEngine;

namespace Db
{
    [CreateAssetMenu(fileName = "RotationCameraParameters", menuName = "Db/RotationCameraParameters")]
    public class RotationCameraParameters : ScriptableObject, IRotationCameraParameters
    {
        [field: SerializeField] public LayerMask AimColliderLayerMask { get; private set; }
        [field: SerializeField] public float RaycastDistance { get; private set; }
        [field: SerializeField] public float RotateSpeed { get; private set; }
        
        [field: SerializeField] public float Sensitivity { get; private set; }
        [field: SerializeField] public float TopClamp { get;  private set; }
        [field: SerializeField] public float BottomClamp { get;  private set; }
        [field: SerializeField] public float AngleOverride { get;  private set; }
    }
}