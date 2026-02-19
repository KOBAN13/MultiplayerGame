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
        private readonly IRemotePlayerParameters _remotePlayerParameters;
        private float _rotationVelocity;

        public SnapshotCharacterMotor(
            ISnapshotsService snapshotsService,
            Transform transform, 
            IRemotePlayerParameters remotePlayerParameters)
        {
            _snapshotsService = snapshotsService;
            _transform = transform;
            _remotePlayerParameters = remotePlayerParameters;
        }

        public void Tick(bool isAim)
        {
            Rotate(isAim);
            Move();
        }

        private void Move()
        {
            var position = _snapshotsService.GetInterpolatedPosition();
            _transform.position = position;
        }

        private void Rotate(bool isAimRotation)
        {
            var yaw = _snapshotsService.GetInterpolatedRotationDirection();

            var rotation = Mathf.SmoothDampAngle(
                _transform.eulerAngles.y,
                yaw,
                ref _rotationVelocity,
                _remotePlayerParameters.RotationSmoothTime
            );

            if (!isAimRotation)
            {
                _transform.rotation = Quaternion.Euler(0f, rotation, 0f);
            }
        }
    }
}
