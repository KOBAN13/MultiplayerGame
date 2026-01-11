using UnityEngine;

namespace Player.Db
{
    public struct ClientStateFrame
    {
        public Vector3 AimDirection;
        public float AimPitch;
        public bool IsGrounded;
        public float RotationY;
        public float Time;
        public int SequenceId;
    }
}
