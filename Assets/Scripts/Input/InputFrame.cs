using UnityEngine;

namespace Input
{
    public struct InputFrame
    {
        public Vector3 Movement;
        public Vector2 Look;
        public Vector3 Origin;
        public Vector3 Direction;
        public bool Jump;
        public bool Run;
        public bool Aim;
        public bool Shoot;
        public float Time;
        public int SequenceId;
    }
}
