using Db.Interface;
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
        private readonly ISnapshotsService _snapshotsService;
        private float _rotationVelocity;

        public RemoteRotationPlayer(
            CharacterController characterController, 
            Transform transform, 
            float rotationSpeed, 
            ISnapshotsService snapshotsService
        )
        {
            _characterController = characterController;
            _transform = transform;
            _rotationSpeed = rotationSpeed;
            _snapshotsService = snapshotsService;
        }
        
        public void RotateCharacter()
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
    }
}