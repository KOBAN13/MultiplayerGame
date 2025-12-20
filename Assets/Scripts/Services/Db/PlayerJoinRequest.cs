using UnityEngine;

namespace Services.Db
{
    public readonly struct PlayerJoinRequest
    {
        public readonly Vector3 Position;
        public readonly string AnimationState;
        public readonly int UserId;

        public PlayerJoinRequest(Vector3 position, string animationState, int userId)
        {
            Position = position;
            AnimationState = animationState;
            UserId = userId;
        }
    }
}