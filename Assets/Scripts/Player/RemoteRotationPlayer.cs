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
        private readonly IRotationCameraParameters  _rotationCameraParameters;
        
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
        
        public void RotateCharacter()
        {
            var moveDirection = _snapshotsService.GetInterpolatedRotationDirection();
            
            if (_characterController.isGrounded)
            {
                if (Vector3.Angle(moveDirection, _transform.forward) > 0)
                {
                    var newDirection = Vector3.RotateTowards(_transform.forward, moveDirection, _rotationSpeed * Time.deltaTime, 0);
                    
                    _transform.rotation = Quaternion.LookRotation(newDirection);
                }
            }
        }

        public void RotateCamera()
        {
            
        }
    }
}