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
        
        protected int PlayerId;
        protected ISnapshotsService SnapshotsService;
        protected IPlayerAnimationState PlayerAnimationState;
        
        private ISnapshotsServiceRegistry _snapshotsServiceRegistry;
        private IPlayerSnapshotReceiver _playerSnapshotReceiver;
        private Func<ISnapshotsServiceRegistry, int, IPlayerSnapshotReceiver> _playerSnapshotReceiverFactory;

        [Inject]
        private void Construct(
            ISnapshotsServiceRegistry snapshotsServiceRegistry,
            Func<ISnapshotsServiceRegistry, int, IPlayerSnapshotReceiver> playerSnapshotReceiverFactory
        )
        {
            _snapshotsServiceRegistry = snapshotsServiceRegistry;
            _playerSnapshotReceiverFactory = playerSnapshotReceiverFactory;
        }

        public void Initialize(int playerId)
        {
            PlayerId = playerId;
            _snapshotsServiceRegistry.AddSnapshotService(playerId);
            SnapshotsService = _snapshotsServiceRegistry.GetSnapshotService(playerId);

            if (SnapshotsService == null)
                throw new InvalidOperationException($"Failed to resolve snapshots service for playerId={playerId}.");

            _playerSnapshotReceiver = _playerSnapshotReceiverFactory(_snapshotsServiceRegistry, playerId);
            OnSnapshotServiceInitialized();
        }

        protected virtual void OnSnapshotServiceInitialized()
        {
        }

        protected void ConfigureAnimator()
        {
            Animator.applyRootMotion = false;
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
