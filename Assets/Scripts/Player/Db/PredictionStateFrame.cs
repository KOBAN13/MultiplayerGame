using UnityEngine;

namespace Player.Db
{
    public struct PredictionStateFrame
    {
        public long InputTick;
        
        public Vector3 Movement;
        public Vector2 Look;
        public Vector3 Origin;
        public Vector3 Direction;
        public bool Jump;
        public bool Run;
        public bool Aim;
        public Vector3 AimDirection;
        public float AimPitch;
        public float RotationY;
    }
}