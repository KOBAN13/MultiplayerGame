using Player.Db;
using UnityEngine;

namespace Services.Interface
{
    public interface ISnapshotsService
    {
        void SyncServerTime(float serverTime);
        void AddSnapshot(ref SnapshotData snapshot);
        Vector3 GetInterpolatedPosition();
        float GetInterpolatedRotationDirection();
    }
}