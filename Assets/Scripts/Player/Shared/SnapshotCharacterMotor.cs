using Db;
using Db.Interface;
using Player.Interface;
using Services.Interface;
using UnityEngine;

namespace Player.Shared
{
    public class SnapshotCharacterMotor : IPlayerSnapshotMotor
    {
        private readonly ISnapshotsService _snapshotsService;
        private readonly CharacterController _characterController;
        private readonly Transform _transform;
        private readonly IPlayerParameters _playerParameters;
        private float _rotationVelocity;

        public SnapshotCharacterMotor(
            ISnapshotsService snapshotsService,
            CharacterController characterController,
            Transform transform, 
            IPlayerParameters playerParameters)
        {
            _snapshotsService = snapshotsService;
            _characterController = characterController;
            _transform = transform;
            _playerParameters = playerParameters;
        }

        public void Tick(bool isAim)
        {
            Rotate(isAim);
            Move();
        }

        private void Move()
        {
            var position = _snapshotsService.GetInterpolatedPosition();
            var direction = (position - _transform.position) * (_playerParameters.SmoothSpeed * Time.deltaTime);
            _characterController.Move(direction);
        }

        private void Rotate(bool isAimRotation)
        {
            if (!_characterController.isGrounded)
                return;

            var yaw = _snapshotsService.GetInterpolatedRotationDirection();

            var rotation = Mathf.SmoothDampAngle(
                _transform.eulerAngles.y,
                yaw,
                ref _rotationVelocity,
                _playerParameters.RotationSmoothTime
            );

            if (!isAimRotation)
            {
                _transform.rotation = Quaternion.Euler(0f, rotation, 0f);
            }
        }
    }
}
