using Db.Interface;
using Player.Interface;
using Services.Interface;
using UnityEngine;

namespace Player.Shared
{
    public class SnapshotCharacterMotor : IPlayerSnapshotMotor
    {
        private readonly ISnapshotsService _snapshotsService;
        private readonly Transform _transform;
        private readonly IPlayerParameters _playerParameters;
        private float _rotationVelocity;

        public SnapshotCharacterMotor(
            ISnapshotsService snapshotsService,
            Transform transform, 
            IPlayerParameters playerParameters)
        {
            _snapshotsService = snapshotsService;
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
            _transform.position =  position;
        }

        private void Rotate(bool isAimRotation)
        {
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
