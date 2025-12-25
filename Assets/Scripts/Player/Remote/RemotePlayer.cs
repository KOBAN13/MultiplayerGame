using System;
using Db.Interface;
using Player.Interface;
using Player.Interface.Local;
using Services.Interface;
using Sfs2X;
using UnityEngine;
using VContainer;

namespace Player.Remote
{
    public class RemotePlayer : MonoBehaviour, IRemotePlayer
    {
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private Animator _animator;
        
        private IPlayerParameters _playerParameters;
        
        private IRemotePlayerMovement _remotePlayerMovement;
        private IRotationComponent _remotePlayerRotation;
        private IPlayerSnapshotReceiver _playerSnapshotReceiver;
        
        private ISnapshotsService _snapshotsService;

        private Func<ISnapshotsService, CharacterController, Transform, float, IRemotePlayerMovement> _remotePlayerMovementFactory;
        private Func<ISnapshotsService, CharacterController, Transform, float, IRotationComponent> _remotePlayerRotationFactory;
        private Func<ISnapshotsService, IPlayerSnapshotReceiver> _playerSnapshotReceiverFactory;
        
        [Inject]
        private void Construct(
            ISnapshotsService snapshotsService, 
            IPlayerParameters playerParameters,
            Func<ISnapshotsService, CharacterController, Transform, float, IRemotePlayerMovement> remotePlayerMovementFactory,
            Func<ISnapshotsService, CharacterController, Transform, float, IRotationComponent> remotePlayerRotationFactory,
            Func<ISnapshotsService, IPlayerSnapshotReceiver> playerSnapshotReceiverFactory
        )
        {
            _snapshotsService = snapshotsService;
            _playerParameters = playerParameters;

            _remotePlayerMovementFactory = remotePlayerMovementFactory;
            _remotePlayerRotationFactory = remotePlayerRotationFactory;
            _playerSnapshotReceiverFactory = playerSnapshotReceiverFactory;

            _remotePlayerMovement = _remotePlayerMovementFactory(
                _snapshotsService,
                _characterController,
                transform,
                _playerParameters.SmoothSpeed);

            _remotePlayerRotation = _remotePlayerRotationFactory(
                _snapshotsService,
                _characterController,
                transform,
                _playerParameters.RotationSmoothTime);

            _playerSnapshotReceiver = _playerSnapshotReceiverFactory(_snapshotsService);
        }

        public void SetAnimationState(string state)
        {
            // TODO: реализовать переключение анимаций
        }

        public void SetSnapshot(Vector3 position, Vector3 inputDirection, float rotation, float serverTime)
        {
            _playerSnapshotReceiver.SetSnapshot(position, inputDirection, rotation, serverTime);
        }

        private void Update()
        {
            _remotePlayerRotation.RotateCharacter();
            
            _remotePlayerMovement.Move();
        }
    }
}
