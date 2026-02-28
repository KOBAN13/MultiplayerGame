using UnityEngine;

namespace Player.Animation
{
    public static class AnimatorHash
    {
        public static readonly int Horizontal = Animator.StringToHash("Horizontal");
        public static readonly int Vertical = Animator.StringToHash("Vertical");
        public static readonly int MoveSpeed = Animator.StringToHash("MoveSpeed");
        public static readonly int StartWalkForward = Animator.StringToHash("StartWalkForward");
        public static readonly int StopWalkForward = Animator.StringToHash("StopWalkForward");
        public static readonly int StartWalkBackward = Animator.StringToHash("StartWalkBackward");
        public static readonly int StopWalkBackward = Animator.StringToHash("StopWalkBackward");
        public static readonly int Jump  = Animator.StringToHash("Jump");
        public static readonly int Fire = Animator.StringToHash("Fire");
        public static readonly int Die = Animator.StringToHash("Die");
    }
}
