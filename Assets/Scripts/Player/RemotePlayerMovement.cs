using Player.Interface;
using Services.Interface;
using UnityEngine;

namespace Player
{
    public class RemotePlayerMovement : IRemotePlayerMovement
    {
        private readonly ISnapshotsService _snapshotsService;
        private readonly CharacterController _characterController;
        private readonly Transform _transform;
        private readonly float _smoothSpeed;
        
        public RemotePlayerMovement(ISnapshotsService snapshotsService,
            CharacterController characterController, 
            Transform transform, 
            float smoothSpeed)
        {
            _snapshotsService = snapshotsService;
            _characterController = characterController;
            _transform = transform;
            _smoothSpeed = smoothSpeed;
        }
        
        public void Move()
        {
            var position = _snapshotsService.GetInterpolatedPosition();
            
            var direction = (position - _transform.position) * (_smoothSpeed * Time.deltaTime);
            
            _characterController.Move(direction);
        }
    }
}