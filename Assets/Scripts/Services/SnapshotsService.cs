using System.Collections.Generic;
using Db.Interface;
using Player.Db;
using Services.Interface;
using UnityEngine;

namespace Services
{
    public class SnapshotsService : ISnapshotsService
    {
        private readonly List<SnapshotData> _snapshots = new();
        private readonly ISnapshotParameters _snapshotParameters;

        private readonly IServerTimeService _serverTimeService;

        public SnapshotsService(ISnapshotParameters snapshotParameters, IServerTimeService serverTimeService)
        {
            _snapshotParameters = snapshotParameters;
            _serverTimeService = serverTimeService;
        }

        public void AddSnapshot(in SnapshotData snapshot)
        {
            if (_snapshots.Count > 0 && snapshot.ServerTime <= _snapshots[^1].ServerTime)
                return;

            _snapshots.Add(snapshot);

            if (_snapshots.Count > _snapshotParameters.MaxBufferSize)
                _snapshots.RemoveAt(0);
        }
        
        public Vector3 GetInterpolatedPosition()
        {
            switch (_snapshots.Count)
            {
                case 0:
                    return Vector3.zero;
                case 1:
                    return _snapshots[0].Position;
            }

            GetInterpolationPair(out var older, out var newer, out var time);

            return Mathf.Approximately(older.ServerTime, newer.ServerTime) 
                ? older.Position 
                : Vector3.Lerp(older.Position, newer.Position, time);
        }
        
        public float GetInterpolatedRotationDirection()
        {
            switch (_snapshots.Count)
            {
                case 0:
                    return 0f;
                case 1:
                    return _snapshots[0].Rotation;
            }

            GetInterpolationPair(out var older, out var newer, out var time);

            return Mathf.Approximately(older.ServerTime, newer.ServerTime) 
                ? older.Rotation 
                : Mathf.LerpAngle(older.Rotation, newer.Rotation, time);
        }

        public (long snapshotId, byte alpha, Vector3 position) GetRenderSnapshotId()
        {
            switch (_snapshots.Count)
            {
                case 0:
                    return (0, 0, Vector3.zero);
                case 1:
                    return (_snapshots[0].SnapshotId, 0, Vector3.zero);
                default:
                    GetInterpolationPair(out var older, out var newer, out var time);
                    var alpha = (byte) Mathf.Clamp(Mathf.RoundToInt(time * 255f), 0, 255);
                    return (older.SnapshotId, alpha, older.Position);
            }
        }

        public bool TryGetInterpolationSnapshots(out SnapshotData older, out SnapshotData newer, out byte alpha)
        {
            older = default;
            newer = default;
            alpha = 0;

            switch (_snapshots.Count)
            {
                case 0:
                    return false;
                case 1:
                    older = _snapshots[0];
                    newer = older;
                    return true;
                default:
                    GetInterpolationPair(out older, out newer, out var time);
                    alpha = (byte)Mathf.Clamp(Mathf.RoundToInt(time * 255f), 0, 255);
                    return true;
            }
        }

        public void SyncServerTime(float serverTime)
        {
            _serverTimeService.SyncServerTime(serverTime);
        }
        
        private float GetServerTime()
        {
            return _serverTimeService.GetServerTime();
        }

        private void GetInterpolationPair(out SnapshotData older, out SnapshotData newer, out float time)
        {
            older = default;
            newer = default;
            time = 0f;

            switch (_snapshots.Count)
            {
                case 0:
                    return;
                case 1:
                    older = _snapshots[0];
                    newer = older;
                    return;
            }

            var serverTime = GetServerTime();
            var interpolationBackTime = serverTime - _snapshotParameters.InterpolationBackTime;

            for (var i = _snapshots.Count - 1; i >= 0; i--)
            {
                if (_snapshots[i].ServerTime > interpolationBackTime && i != 0)
                    continue;

                older = _snapshots[i];
                newer = i < _snapshots.Count - 1 ? _snapshots[i + 1] : older;

                if (Mathf.Approximately(older.ServerTime, newer.ServerTime))
                    return;

                time = Mathf.InverseLerp(older.ServerTime, newer.ServerTime, interpolationBackTime);
                return;
            }

            older = _snapshots[^1];
            newer = older;
        }
    }
}
