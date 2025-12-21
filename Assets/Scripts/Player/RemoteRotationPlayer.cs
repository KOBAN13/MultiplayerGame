using Db.Interface;
using Player.Interface;
using Services.Interface;
using UnityEngine;

namespace Player
{
    public class RemoteRotationPlayer : IRotationComponent
    {
        private readonly CharacterController _characterController;
        private readonly Transform _transform;
        private readonly float _rotationSpeed;
        private readonly ISnapshotsService _snapshotsService;
        private readonly IRotationCameraParameters _rotationCameraParameters;

        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;
        private float _rotationVelocity;
        private const float THRESHOLD = 0.01f;

        public RemoteRotationPlayer(
            CharacterController characterController, 
            Transform transform, 
            float rotationSpeed, 
            ISnapshotsService snapshotsService, 
            IRotationCameraParameters rotationCameraParameters
        )
        {
            _characterController = characterController;
            _transform = transform;
            _rotationSpeed = rotationSpeed;
            _snapshotsService = snapshotsService;
            _rotationCameraParameters = rotationCameraParameters;
        }
        
        public void RotateCharacter(Transform cameraTransform)
        {
            if (!_characterController.isGrounded)
                return;

            var yaw = _snapshotsService.GetInterpolatedRotationDirection();

            var rotation = Mathf.SmoothDampAngle(
                _transform.eulerAngles.y,
                yaw,
                ref _rotationVelocity,
                _rotationSpeed
            );

            _transform.rotation = Quaternion.Euler(0f, rotation, 0f);
        }

        public Quaternion RotateCamera(Vector2 position)
        {
            var bottomClamp = _rotationCameraParameters.BottomClamp;
            var topClamp = _rotationCameraParameters.TopClamp;
            var cameraAngleOverride = _rotationCameraParameters.AngleOverride;
            var sensitivity = _rotationCameraParameters.Sensitivity;
            
            if (position.sqrMagnitude >= THRESHOLD)
            {
                _cinemachineTargetYaw += position.x * Time.deltaTime * sensitivity;
                _cinemachineTargetPitch += position.y * Time.deltaTime * sensitivity;
            }
            
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, bottomClamp, topClamp);
            
            return Quaternion.Euler(_cinemachineTargetPitch + cameraAngleOverride, _cinemachineTargetYaw, 0.0f);
        }
        
        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) 
                lfAngle += 360f;
            
            if (lfAngle > 360f) 
                lfAngle -= 360f;
            
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }
    }
}