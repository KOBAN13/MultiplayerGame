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
        
        public void SetSnapshot(in SnapshotData snapshotData)
        {
            _snapshotsService.SyncServerTime(snapshotData.ServerTime);
            
            _snapshotsService.AddSnapshot(snapshotData);
        }
    }
}