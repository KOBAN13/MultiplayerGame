using UnityEngine;

namespace Input
{
    public struct InputFrame
    {
        public long InputTick;
        
        public Vector3 Movement;
        public Vector3 AimDirection;
        public bool Jump;
        public bool Run;
        public bool Aim;
        public float AimPitch;
        public float RotationY;
    }
}
