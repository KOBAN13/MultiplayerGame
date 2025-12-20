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
        
        private IPlayerParameters _playerParameters;
        
        private IRemotePlayerMovement _remotePlayerMovement;
        private IRotationComponent _remotePlayerRotation;
        private IPlayerSnapshotReceiver _playerSnapshotReceiver;
        private IPlayerNetworkInputSender _playerNetworkInputSender;
        
        private IPlayerNetworkInputReader _playerNetworkInputReader;
        private ISnapshotsService _snapshotsService;
        private SmartFox _sfs;
        
        [Inject]
        private void Construct(IPlayerNetworkInputReader playerNetworkInputReader, SmartFox sfs, ISnapshotsService snapshotsService, IPlayerParameters playerParameters)
        {
            _snapshotsService = snapshotsService;
            _playerNetworkInputReader = playerNetworkInputReader;
            _sfs = sfs;
            _playerParameters = playerParameters;
            
            _remotePlayerMovement 
                = new RemotePlayerMovement(_snapshotsService, _characterController, transform, _playerParameters.SmoothSpeed);
            
            _remotePlayerRotation 
                = new RemoteRotationPlayer(_characterController, transform, _playerParameters.RotationSpeed, _snapshotsService);
            
            _playerSnapshotReceiver
                = new PlayerSnapshotReceiver(_snapshotsService);
            
            _playerNetworkInputSender 
                = new PlayerNetworkInputSender(_playerNetworkInputReader, _sfs, _characterController);
        }
        
        public void SetAnimationState(string state)
        {
            // TODO: реализовать переключение анимаций
        }

        public void SetSnapshot(Vector3 position, Vector3 rotationDirection, float serverTime)
        {
            _playerSnapshotReceiver.SetSnapshot(position, rotationDirection, serverTime);
        }

        public Transform GetTransform() => transform;
        
        private void Update()
        {
            _remotePlayerMovement.Move();
            _remotePlayerRotation.RotateCharacter();
        }
        
        private void OnDestroy()
        {
            _playerNetworkInputSender.Dispose();
        }

    }
}