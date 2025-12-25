using Player.Utils;
using UnityEngine;

namespace Services.Db
{
    public readonly struct PlayerJoinRequest
    {
        public readonly EPlayerType PlayerType;
        public readonly Vector3 Position;
        public readonly string AnimationState;
        public readonly int UserId;

        public PlayerJoinRequest(Vector3 position, string animationState, int userId, EPlayerType playerType)
        {
            Position = position;
            AnimationState = animationState;
            UserId = userId;
            PlayerType = playerType;
        }
    }
}