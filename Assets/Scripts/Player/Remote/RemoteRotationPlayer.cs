using Player.Interface;
using Services.Interface;
using UnityEngine;

namespace Player.Remote
{
    public class RemoteRotationPlayer : IRotationComponent
    {
        private readonly CharacterController _characterController;
        private readonly Transform _transform;
        private readonly float _rotationSpeed;
        private readonly ISnapshotsServiceRegistry _snapshotsServiceRegistry;
        private readonly int _playerId;
        private float _rotationVelocity;

        public RemoteRotationPlayer(
            CharacterController characterController, 
            Transform transform, 
            float rotationSpeed, 
            ISnapshotsServiceRegistry snapshotsServiceRegistry,
            int playerId
        )
        {
            _characterController = characterController;
            _transform = transform;
            _rotationSpeed = rotationSpeed;
            _snapshotsServiceRegistry = snapshotsServiceRegistry;
            _playerId = playerId;
        }
        
        public void RotateCharacter()
        {
            if (!_characterController.isGrounded)
                return;

            var snapshotsService = _snapshotsServiceRegistry.GetSnapshotService(_playerId);
            if (snapshotsService == null)
                return;

            var yaw = snapshotsService.GetInterpolatedRotationDirection();

            var rotation = Mathf.SmoothDampAngle(
                _transform.eulerAngles.y,
                yaw,
                ref _rotationVelocity,
                _rotationSpeed
            );

            _transform.rotation = Quaternion.Euler(0f, rotation, 0f);
        }
    }
}
