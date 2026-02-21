using Player.Db;
using UnityEngine;

namespace Services.Interface
{
    public interface ISnapshotsService
    {
        AimData GetAimData();
        void SyncServerTime(float serverTime);
        void AddSnapshot(in SnapshotData snapshot);
        Vector3 GetInterpolatedPosition();
        float GetInterpolatedRotationDirection();
        (long snapshotId, byte alpha) GetRenderSnapshotId();
    }
}
