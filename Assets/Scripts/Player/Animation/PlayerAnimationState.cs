using Player.Db;
using UnityEngine;

namespace Player.Animation
{
    public class PlayerAnimationState : IPlayerAnimationState
    {
        private readonly Animator _animator;
        
        private const float ANIMATION_DAMP_TIME = 0.1f;
        private const float DIRECTION_THRESHOLD = 0.1f;
        
        private PlayerStateData _lastAnimationState;
        private float _lastNonZeroVertical = 1f;
        private bool _hasAnimationState;

        public PlayerAnimationState(Animator animator)
        {
            _animator = animator;
        }

        public void UpdateMovementAnimation(Vector3 localDirection, float moveSpeed, in PlayerStateData playerState, float deltaTime)
        {
            var planarDirection = new Vector3(localDirection.x, 0f, localDirection.z);
            
            if (planarDirection.sqrMagnitude > 1f)
                planarDirection.Normalize();

            var horizontal = 0f;
            var vertical = 0f;
            
            var normalizedSpeed = Mathf.Clamp01(moveSpeed);

            if (planarDirection.sqrMagnitude > 0.0001f)
            {
                horizontal = Mathf.Clamp(planarDirection.x, -1f, 1f);
                vertical = Mathf.Clamp(planarDirection.z, -1f, 1f);
            }

            if (normalizedSpeed <= 0.001f && planarDirection.sqrMagnitude <= 0.0001f)
            {
                horizontal = 0f;
                vertical = 0f;
                normalizedSpeed = 0f;
            }

            _animator.SetFloat(AnimatorHash.Horizontal, horizontal, ANIMATION_DAMP_TIME, deltaTime);
            _animator.SetFloat(AnimatorHash.Vertical, vertical, ANIMATION_DAMP_TIME, deltaTime);
            _animator.SetFloat(AnimatorHash.MoveSpeed, normalizedSpeed, ANIMATION_DAMP_TIME, deltaTime);

            UpdateAnimationTriggers(vertical, in playerState);

            if (Mathf.Abs(vertical) > DIRECTION_THRESHOLD)
                _lastNonZeroVertical = Mathf.Sign(vertical);

            _lastAnimationState = playerState;
            _hasAnimationState = true;
        }

        private void UpdateAnimationTriggers(float vertical, in PlayerStateData playerState)
        {
            if (ShouldTrigger(playerState.IsStartRun, _lastAnimationState.IsStartRun))
                TriggerDirectional(AnimatorHash.StartWalkForward, AnimatorHash.StartWalkBackward, vertical);

            if (ShouldTrigger(playerState.IsStopRun, _lastAnimationState.IsStopRun))
                TriggerDirectional(AnimatorHash.StopWalkForward, AnimatorHash.StopWalkBackward, vertical);

            if (ShouldTrigger(playerState.IsJump, _lastAnimationState.IsJump))
                _animator.SetTrigger(AnimatorHash.Jump);

            if (ShouldTrigger(playerState.IsShot, _lastAnimationState.IsShot))
                _animator.SetTrigger(AnimatorHash.Fire);

            if (ShouldTrigger(playerState.IsDead, _lastAnimationState.IsDead))
                _animator.SetTrigger(AnimatorHash.Die);
        }

        private bool ShouldTrigger(bool currentValue, bool previousValue)
        {
            return currentValue && (!_hasAnimationState || !previousValue);
        }

        private void TriggerDirectional(int forwardHash, int backwardHash, float vertical)
        {
            var resolvedVertical = Mathf.Abs(vertical) > DIRECTION_THRESHOLD
                ? vertical
                : _lastNonZeroVertical;

            if (Mathf.Abs(resolvedVertical) <= DIRECTION_THRESHOLD)
                return;

            _animator.SetTrigger(resolvedVertical >= 0f ? forwardHash : backwardHash);
        }

        public Vector3 ToLocalAnimationDirection(Vector3 worldDirection, Quaternion referenceRotation)
        {
            var localDirection = Quaternion.Inverse(referenceRotation) * new Vector3(worldDirection.x, 0f, worldDirection.z);
            localDirection.y = 0f;

            return localDirection;
        }

        public float NormalizeAnimationSpeed(float speed, float maxSpeed)
        {
            return Mathf.Clamp01(speed / Mathf.Max(maxSpeed, 0.0001f));
        }

        public float NormalizeAnimationSpeed(float speed)
        {
            return Mathf.Clamp01(speed);
        }

        public PlayerStateData BuildAnimationState(
            Vector3 input,
            float velocity,
            bool isGrounded,
            bool isStartRun,
            bool isStopRun,
            bool isShot,
            bool isJump,
            bool isDead)
        {
            return new PlayerStateData
            {
                Input = input,
                Velocity = velocity,
                IsGrounded = isGrounded,
                IsStartRun = isStartRun,
                IsStopRun = isStopRun,
                IsShot = isShot,
                IsJump = isJump,
                IsDead = isDead
            };
        }
    }
}