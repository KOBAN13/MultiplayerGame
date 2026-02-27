using UnityEngine;

namespace Player.Db
{
    public struct PredictionStateFrame
    {
        public long InputTick;
        
        public Vector3 Position;
        public Vector3 Velocity;
        public Vector3 MoveDirection;
        public float Rotation;
        public bool IsGrounded;
        public int AnimationState;
    }
}
