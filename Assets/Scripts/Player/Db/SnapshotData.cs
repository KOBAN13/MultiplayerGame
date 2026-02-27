using UnityEngine;

namespace Player.Db
{
    public struct SnapshotData
    {
        public AimData AimData;
        
        public Vector3 Position;
        public Vector3 Input;
        public float Rotation;
        public bool IsGrounded;
        public int AnimationState;
        public long SnapshotId;
        public long LastProcessedInputSequence;
        public float ServerTime;
    }
}
