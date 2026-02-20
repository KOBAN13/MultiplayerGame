using Player.Interface;
using Services.Interface;
using UnityEngine;

namespace Player.Remote
{
    public class RemotePlayerMovement : IRemotePlayerMovement
    {
        private readonly CharacterController _characterController;
        private readonly Transform _transform;
        private readonly float _smoothSpeed;
        private readonly ISnapshotsService _snapshotsService;
        
        public RemotePlayerMovement(
            ISnapshotsServiceRegistry snapshotsServiceRegistry,
            int playerId,
            CharacterController characterController, 
            Transform transform, 
            float smoothSpeed)
        {
            _characterController = characterController;
            _transform = transform;
            _smoothSpeed = smoothSpeed;
            
            _snapshotsService = snapshotsServiceRegistry.GetSnapshotService(playerId);
        }
        
        public void Move()
        {
            var position = _snapshotsService.GetInterpolatedPosition();
            
            var direction = (position - _transform.position) * (_smoothSpeed * Time.deltaTime);
            
            _characterController.Move(direction);
        }
    }
}
