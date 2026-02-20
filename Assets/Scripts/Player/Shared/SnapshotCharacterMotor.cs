using Db.Interface;
using Player.Interface;
using Services.Interface;
using UnityEngine;

namespace Player.Shared
{
    public class SnapshotCharacterMotor : IPlayerSnapshotMotor
    {
        private readonly Transform _transform;
        private readonly IRemotePlayerParameters _remotePlayerParameters;
        private float _rotationVelocity;
        
        private readonly ISnapshotsService _snapshotsService;

        public SnapshotCharacterMotor(
            ISnapshotsServiceRegistry snapshotsServiceRegistry,
            int playerId,
            Transform transform, 
            IRemotePlayerParameters remotePlayerParameters)
        {
            _transform = transform;
            _remotePlayerParameters = remotePlayerParameters;
            
            _snapshotsService = snapshotsServiceRegistry.GetSnapshotService(playerId);
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
