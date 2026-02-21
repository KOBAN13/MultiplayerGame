using Db.Interface;
using Player.Db;
using Player.Interface;
using Services.Interface;
using UnityEngine;

namespace Player.Shared
{
    public class SnapshotCharacterMotor : IPlayerSnapshotMotor
    {
        private readonly Transform _visualRoot;
        private readonly Transform _physicalRoot;
        private readonly IRemotePlayerParameters _remotePlayerParameters;
        private float _rotationVelocity;
        
        private readonly ISnapshotsService _snapshotsService;

        public SnapshotCharacterMotor(
            ISnapshotsServiceRegistry snapshotsServiceRegistry,
            int playerId,
            Transform visualRoot, 
            IRemotePlayerParameters remotePlayerParameters, 
            Transform physicalRoot)
        {
            _visualRoot = visualRoot;
            _remotePlayerParameters = remotePlayerParameters;
            _physicalRoot = physicalRoot;

            _snapshotsService = snapshotsServiceRegistry.GetSnapshotService(playerId);
        }

        public void Tick()
        {
            RemotePlayerRotation();
            
            Move();
        }

        private void Move()
        {
            var position = _snapshotsService.GetInterpolatedPosition();
            _visualRoot.position = position;
        }
        
        private void RemotePlayerRotation()
        {
            var aimData = _snapshotsService.GetAimData();
            
            if (aimData.isAim)
                AimRotate(aimData);
            else
                Rotate();
        }

        private void Rotate()
        {
            var yaw = _snapshotsService.GetInterpolatedRotationDirection();

            var rotation = Mathf.SmoothDampAngle(
                _visualRoot.eulerAngles.y,
                yaw,
                ref _rotationVelocity,
                _remotePlayerParameters.RotationSmoothTime
            );
            
            _visualRoot.rotation = Quaternion.Euler(0f, rotation, 0f);
        }
        
        private void AimRotate(in AimData data)
        {
            var direction = data.AimDirection;
            direction.y = 0f;

            if (direction.sqrMagnitude < 0.0001f)
                return;

            var targetRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
            PhysicalRoot.rotation = Quaternion.Slerp(PhysicalRoot.rotation, targetRotation, Time.deltaTime * _remotePlayerParameters.RotateSpeed);
            
            var cameraAngleOverride = _remotePlayerParameters.AngleOverride;
            
            var yawRotation = Quaternion.Euler(data.AimPitch + cameraAngleOverride, 0.0f, 0.0f);
            
            _yawTarget.localRotation = Quaternion.Slerp(
                _yawTarget.localRotation,
                yawRotation,
                Time.deltaTime * _remotePlayerParameters.RotateSpeed);
        }
    }
}
