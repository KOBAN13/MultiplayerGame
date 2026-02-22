using Db.Interface;
using Player.Db;
using Player.Interface;
using Services.Interface;
using UnityEngine;

namespace Player.Shared
{
    public class SnapshotCharacterMotor : IPlayerSnapshotMotor
    {
        private readonly SnapshotCharacterMotorArgs _args;
        
        private float _rotationVelocity;

        private readonly ISnapshotsService _snapshotsService;
        private readonly Transform _physicalRoot;
        private readonly Transform _visualRoot;
        private readonly Transform _yawTarget;
        private readonly IRemotePlayerParameters _remotePlayerParameters;

        public SnapshotCharacterMotor(SnapshotCharacterMotorArgs args)
        {
            _snapshotsService = args._snapshotsService;
            _remotePlayerParameters = args._remotePlayerParameters;
            _physicalRoot = args._physicalRoot;
            _visualRoot = args._visualRoot;
            _yawTarget = args._yawTarget;
            _args = args;
        }

        public void Tick()
        {
            RemotePlayerRotation();
            Move();
        }

        private void Move()
        {
            var position = _snapshotsService.GetInterpolatedPosition();
            _physicalRoot.position = position;
        }
        
        private void RemotePlayerRotation()
        {
            var aimData = _snapshotsService.GetAimData();

            if (aimData.isAim)
            {
                AimRotate(aimData);
                return;
            }

            Rotate();
            ResetYawTarget();
        }

        private void Rotate()
        {
            var isGrounded = _snapshotsService.GetGroundedParam();
            
            if (!isGrounded)
                return;
            
            var yaw = _snapshotsService.GetInterpolatedRotationDirection();

            var rotation = Mathf.SmoothDampAngle(
                _physicalRoot.eulerAngles.y,
                yaw,
                ref _rotationVelocity,
                _remotePlayerParameters.RotationSmoothTime
            );

            _physicalRoot.rotation = Quaternion.Euler(0f, rotation, 0f);
        }

        private void AimRotate(in AimData data)
        {
            var direction = data.AimDirection;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.0001f)
            {
                var targetRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);

                _physicalRoot.rotation = Quaternion.Slerp(
                    _physicalRoot.rotation,
                    targetRotation,
                    Time.deltaTime * _remotePlayerParameters.RotateSpeed);
            }
            
            var cameraAngleOverride = _remotePlayerParameters.AngleOverride;
            var yawRotation = Quaternion.Euler(data.AimPitch + cameraAngleOverride, 0.0f, 0.0f);

            _yawTarget.localRotation = Quaternion.Slerp(
                _yawTarget.localRotation,
                yawRotation,
                Time.deltaTime * _remotePlayerParameters.RotateSpeed);
        }

        private void ResetYawTarget()
        {
            _yawTarget.localRotation = Quaternion.Slerp(
                _yawTarget.localRotation,
                Quaternion.identity,
                Time.deltaTime * _remotePlayerParameters.RotateSpeed);
        }
    }
}
