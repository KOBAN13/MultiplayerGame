using Player.Db;
using Player.Interface.Local;
using Services.Interface;
using UnityEngine;

namespace Player.Local
{
    public class PlayerSnapshotReceiver : IPlayerSnapshotReceiver
    {
        private readonly ISnapshotsService _snapshotsService;
        
        public PlayerSnapshotReceiver(ISnapshotsService snapshotsService)
        {
            _snapshotsService = snapshotsService;
        }
        
        public void SetSnapshot(Vector3 position, Vector3 inputDirection, float rotation, float serverTime)
        {
            var snapshot = new SnapshotData
            {
                Position = position,
                Input = inputDirection,
                Rotation = rotation,
                ServerTime = serverTime
            };
            
            _snapshotsService.SyncServerTime(serverTime);
            
            _snapshotsService.AddSnapshot(ref snapshot);
        }
    }
}