using System;
using Db.Interface;
using Player.Interface;
using Services.Interface;
using UnityEngine;
using VContainer;

namespace Player
{
    public abstract class APlayer : MonoBehaviour
    {
        [SerializeField] protected CharacterController CharacterController;
        [SerializeField] protected Animator Animator;
        
        protected ISnapshotsService SnapshotsService;
        protected IPlayerSnapshotMotor SnapshotMotor;
        protected IPlayerSnapshotReceiver PlayerSnapshotReceiver;
        protected IPlayerParameters PlayerParameters;
        
        private Func<ISnapshotsService, CharacterController, Transform, IPlayerParameters, IPlayerSnapshotMotor> _snapshotMotorFactory;
        private Func<ISnapshotsService, IPlayerSnapshotReceiver> _playerSnapshotReceiverFactory;

        [Inject]
        private void Construct(
            ISnapshotsService snapshotsService, 
            IPlayerParameters playerParameters,
            Func<ISnapshotsService, CharacterController, Transform, IPlayerParameters, IPlayerSnapshotMotor> snapshotMotorFactory,
            Func<ISnapshotsService, IPlayerSnapshotReceiver> playerSnapshotReceiverFactory
        )
        {
            SnapshotsService = snapshotsService;
            PlayerParameters = playerParameters;

            _snapshotMotorFactory = snapshotMotorFactory;
            _playerSnapshotReceiverFactory = playerSnapshotReceiverFactory;

            SnapshotMotor = _snapshotMotorFactory(
                SnapshotsService,
                CharacterController,
                transform, 
                playerParameters);

            PlayerSnapshotReceiver = _playerSnapshotReceiverFactory(SnapshotsService);
        }
        
        public virtual void SetAnimationState(string state)
        {
            // TODO: реализовать переключение анимаций
        }

        public virtual void SetSnapshot(Vector3 position, Vector3 inputDirection, float rotation, float serverTime)
        {
            PlayerSnapshotReceiver.SetSnapshot(position, inputDirection, rotation, serverTime);
        }

        private void Update()
        {
            SnapshotMotor.Tick();
        }
    }
}