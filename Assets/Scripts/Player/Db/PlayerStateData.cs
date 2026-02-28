using UnityEngine;

namespace Player.Db
{
    public struct PlayerStateData
    {
        public Vector3 Input;
        public Vector3 Position;
        public float Velocity;
        
        public bool IsStartRun;
        public bool IsStopRun;
        public bool IsShot;
        public bool IsJump;
        public bool IsGrounded;
        public bool IsDead;
    }
}