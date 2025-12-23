using System;
using Db.Interface;
using Input;
using Player.Interface;
using Services.Interface;
using Sfs2X;
using UnityEngine;
using VContainer;

namespace Player
{
    public class RemotePlayer : MonoBehaviour, IRemotePlayer
    {
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _cameraTarget;
        
        private IPlayerParameters _playerParameters;
        
        private IRemotePlayerMovement _remotePlayerMovement;
        private IRotationComponent _remotePlayerRotation;
        private IPlayerSnapshotReceiver _playerSnapshotReceiver;
        private IPlayerNetworkInputSender _playerNetworkInputSender;
        
        private IInputSource _inputSource;
        private ISnapshotsService _snapshotsService;
        private SmartFox _sfs;
        private InputFrame _lastInputFrame;

        private Func<ISnapshotsService, CharacterController, Transform, float, IRemotePlayerMovement> _remotePlayerMovementFactory;
        private Func<ISnapshotsService, CharacterController, Transform, float, IRotationComponent> _remotePlayerRotationFactory;
        private Func<ISnapshotsService, IPlayerSnapshotReceiver> _playerSnapshotReceiverFactory;
        private Func<SmartFox, CharacterController, Transform, IPlayerNetworkInputSender> _playerNetworkInputSenderFactory;
        
        [Inject]
        private void Construct(
            IInputSource inputSource, 
            SmartFox sfs, 
            ISnapshotsService snapshotsService, 
            IPlayerParameters playerParameters,
            Func<ISnapshotsService, CharacterController, Transform, float, IRemotePlayerMovement> remotePlayerMovementFactory,
            Func<ISnapshotsService, CharacterController, Transform, float, IRotationComponent> remotePlayerRotationFactory,
            Func<ISnapshotsService, IPlayerSnapshotReceiver> playerSnapshotReceiverFactory,
            Func<SmartFox, CharacterController, Transform, IPlayerNetworkInputSender> playerNetworkInputSenderFactory
        )
        {
            _snapshotsService = snapshotsService;
            _inputSource = inputSource;
            _sfs = sfs;
            _playerParameters = playerParameters;

            _remotePlayerMovementFactory = remotePlayerMovementFactory;
            _remotePlayerRotationFactory = remotePlayerRotationFactory;
            _playerSnapshotReceiverFactory = playerSnapshotReceiverFactory;
            _playerNetworkInputSenderFactory = playerNetworkInputSenderFactory;

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

            _playerNetworkInputSender = _playerNetworkInputSenderFactory(
                _sfs,
                _characterController,
                _cameraTarget);
        }

        public Transform GetCameraTarget()
        {
            return _cameraTarget;
        }

        public void SetAnimationState(string state)
        {
            // TODO: реализовать переключение анимаций
        }

        public void SetSnapshot(Vector3 position, Vector3 inputDirection, float rotation, float serverTime)
        {
            _playerSnapshotReceiver.SetSnapshot(position, inputDirection, rotation, serverTime);
        }

        public Transform GetTransform() => transform;

        private void Update()
        {
            _lastInputFrame = _inputSource.Read();
            _playerNetworkInputSender.SendServerPlayerInput(_lastInputFrame);
            
            _remotePlayerRotation.RotateCharacter();
            
            _remotePlayerMovement.Move();
        }

        private void LateUpdate()
        {
            _cameraTarget.rotation = _remotePlayerRotation.RotateCamera(_lastInputFrame.Look);
        }

        private void OnDestroy()
        {
            _playerNetworkInputSender.Dispose();
        }
    }
}
