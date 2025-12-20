using Player.Db;
using Player.Interface;
using Services.Interface;
using UnityEngine;

namespace Player
{
    public class PlayerSnapshotReceiver : IPlayerSnapshotReceiver
    {
        private readonly ISnapshotsService _snapshotsService;
        
        public PlayerSnapshotReceiver(ISnapshotsService snapshotsService)
        {
            _snapshotsService = snapshotsService;
        }
        
        public void SetSnapshot(Vector3 position, Vector3 rotationDirection, float serverTime)
        {
            var snapshot = new SnapshotData
            {
                Position = position,
                RotationDirection = rotationDirection,
                ServerTime = serverTime
            };
            
            _snapshotsService.SyncServerTime(serverTime);
            
            _snapshotsService.AddSnapshot(ref snapshot);
        }
    }
}