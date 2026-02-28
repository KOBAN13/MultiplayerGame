using Player.Db;
using UnityEngine;

namespace Player.Animation
{
    public interface IPlayerAnimationState
    {
        void UpdateMovementAnimation(Vector3 localDirection, float moveSpeed, in PlayerStateData playerState,
            float deltaTime);

        Vector3 ToLocalAnimationDirection(Vector3 worldDirection, Quaternion referenceRotation);

        float NormalizeAnimationSpeed(float speed, float maxSpeed);

        float NormalizeAnimationSpeed(float speed);

        PlayerStateData BuildAnimationState(
            Vector3 input,
            float velocity,
            bool isGrounded,
            bool isStartRun,
            bool isStopRun,
            bool isShot,
            bool isJump,
            bool isDead);
    }
}