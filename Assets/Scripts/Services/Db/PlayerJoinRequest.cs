using UnityEngine;
using Utils.Enums;

namespace Services.Db
{
    public readonly struct PlayerJoinRequest
    {
        public readonly EPlayerType PlayerType;
        public readonly Vector3 Position;
        public readonly int AnimationState;
        public readonly int UserId;

        public PlayerJoinRequest(Vector3 position, int animationState, int userId, EPlayerType playerType)
        {
            Position = position;
            AnimationState = animationState;
            UserId = userId;
            PlayerType = playerType;
        }
    }
}