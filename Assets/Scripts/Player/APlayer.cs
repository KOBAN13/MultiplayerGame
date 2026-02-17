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
        
        protected ISnapshotsService SnapshotsService;
        protected IPlayerSnapshotReceiver PlayerSnapshotReceiver;

        [Inject]
        private void Construct(
            ISnapshotsService snapshotsService, 
            Func<ISnapshotsService, IPlayerSnapshotReceiver> playerSnapshotReceiverFactory
        )
        {
            SnapshotsService = snapshotsService;
            PlayerSnapshotReceiver = playerSnapshotReceiverFactory(SnapshotsService);
        }
        
        public virtual void SetAnimationState(string state)
        {
            // TODO: реализовать переключение анимаций
        }

        public virtual void SetSnapshot(in SnapshotData snapshot)
        {
            PlayerSnapshotReceiver.SetSnapshot(snapshot);
        }
    }
}