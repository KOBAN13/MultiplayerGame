using UnityEngine;

namespace Player.Db
{
    public struct PredictionStateFrame
    {
        public long InputTick;
        
        public Vector3 Position;
        public float Rotation;
        public bool IsGrounded;
        public string AnimationState;
    }
}