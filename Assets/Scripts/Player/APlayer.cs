using System;
using Player.Db;
using Player.Interface.Local;
using Services.Interface;
using UnityEngine;
using VContainer;

namespace Player
{
    public abstract class APlayer : MonoBehaviour
    {
        [SerializeField] protected Animator Animator;
        [SerializeField] protected Transform YawTarget;
        
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
        
        public virtual void SetAnimationState(string state)
        {
            // TODO: реализовать переключение анимаций
        }

        public virtual void SetSnapshot(in SnapshotData snapshot)
        {
            _playerSnapshotReceiver.SetSnapshot(snapshot);
        }
    }
}
