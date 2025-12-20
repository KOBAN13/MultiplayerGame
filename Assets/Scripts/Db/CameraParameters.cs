using Db.Interface;
using UnityEngine;

namespace Db
{
    [CreateAssetMenu(fileName = "CameraParameters", menuName = "Db/CameraParameters")]
    public class CameraParameters : ScriptableObject, ICameraParameters
    {
        [field: SerializeField] public int CameraFOV { get; private set; }
        
        [field: SerializeField] public Vector3 Damping { get; private set; }
        
        [field: SerializeField] public Vector3 ShoulderOffset { get; private set; }
        
        [field: SerializeField]
        [field: Range(0, 1)]
        public float CameraSide { get; private set; }
        
        [field: SerializeField] public float CameraDistance { get; private set; }
    }
}