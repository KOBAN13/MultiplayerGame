using System;
using Player.Animation;
using Player.Db;
using Player.Interface.Local;
using Player.Weapon;
using Services.Interface;
using UnityEngine;
using VContainer;

namespace Player
{
    public abstract class APlayer : MonoBehaviour
    {
        [SerializeField] protected Animator Animator;
        [SerializeField] protected Transform YawTarget;
        [SerializeField] protected AWeapon CurrentWeapon;

        private const float AnimationDampTime = 0.1f;
        
        protected int PlayerId;
        protected ISnapshotsService SnapshotsService;
        protected ISnapshotsServiceRegistry SnapshotsServiceRegistry { get; private set; }

        private IPlayerSnapshotReceiver _playerSnapshotReceiver;
        private Func<ISnapshotsServiceRegistry, int, IPlayerSnapshotReceiver> _playerSnapshotReceiverFactory;

        [Inject]
        private void Construct(
            ISnapshotsServiceRegistry snapshotsServiceRegistry,
            Func<ISnapshotsServiceRegistry, int, IPlayerSnapshotReceiver> playerSnapshotReceiverFactory
        )
        {
            SnapshotsServiceRegistry = snapshotsServiceRegistry;
            _playerSnapshotReceiverFactory = playerSnapshotReceiverFactory;
        }

        public void Initialize(int playerId)
        {
            PlayerId = playerId;
            SnapshotsServiceRegistry.AddSnapshotService(playerId);
            SnapshotsService = SnapshotsServiceRegistry.GetSnapshotService(playerId);

            if (SnapshotsService == null)
                throw new InvalidOperationException($"Failed to resolve snapshots service for playerId={playerId}.");

            _playerSnapshotReceiver = _playerSnapshotReceiverFactory(SnapshotsServiceRegistry, playerId);
            OnSnapshotServiceInitialized();
        }

        protected virtual void OnSnapshotServiceInitialized()
        {
        }

        protected void ConfigureAnimator()
        {
            if (Animator == null)
                return;

            // Character motion is driven by motor/network code, not by root motion in clips.
            Animator.applyRootMotion = false;
        }

        protected void UpdateMovementAnimation(Vector3 worldDirection, Quaternion referenceRotation, float deltaTime)
        {
            if (Animator == null)
                return;

            var planarDirection = new Vector3(worldDirection.x, 0f, worldDirection.z);
            var magnitude = Mathf.Clamp01(planarDirection.magnitude);

            if (magnitude <= 0.001f)
            {
                Animator.SetFloat(AnimatorHash.Horizontal, 0f, AnimationDampTime, deltaTime);
                Animator.SetFloat(AnimatorHash.Vertical, 0f, AnimationDampTime, deltaTime);
                Animator.SetFloat(AnimatorHash.MoveSpeed, 0f, AnimationDampTime, deltaTime);
                return;
            }

            var localDirection = Quaternion.Inverse(referenceRotation) * (planarDirection / magnitude);
            var horizontal = Mathf.Clamp(localDirection.x * magnitude, -1f, 1f);
            var vertical = Mathf.Clamp(localDirection.z * magnitude, -1f, 1f);

            Animator.SetFloat(AnimatorHash.Horizontal, horizontal, AnimationDampTime, deltaTime);
            Animator.SetFloat(AnimatorHash.Vertical, vertical, AnimationDampTime, deltaTime);
            Animator.SetFloat(AnimatorHash.MoveSpeed, magnitude, AnimationDampTime, deltaTime);
        }
        
        public virtual void SetAnimationState(int state)
        {
            //Animator.SetInteger(state, 1);
        }

        public virtual void SetSnapshot(in SnapshotData snapshot)
        {
            _playerSnapshotReceiver.SetSnapshot(snapshot);
        }
    }
}
