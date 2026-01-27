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

        private const float POSITION_EPSILON_SQR = 0.0001f;
        private const float ROTATION_EPSILON = 0.1f;

        private bool _hasServerTime;
        private float _serverTimeOffset;

        public SnapshotsService(ISnapshotParameters snapshotParameters)
        {
            _snapshotParameters = snapshotParameters;
        }

        public void AddSnapshot(in SnapshotData snapshot)
        {
            if (_snapshots.Count > 0)
            {
                var lastSnapshotIndex = _snapshots.Count - 1;
                var lastSnapshot = _snapshots[lastSnapshotIndex];
                
                if (snapshot.ServerTime <= lastSnapshot.ServerTime)
                    return;
                
                if ((snapshot.Position - lastSnapshot.Position).sqrMagnitude < POSITION_EPSILON_SQR)
                {
                    if (Mathf.Abs(Mathf.DeltaAngle(lastSnapshot.Rotation, snapshot.Rotation)) < ROTATION_EPSILON)
                    {
                        lastSnapshot.ServerTime = snapshot.ServerTime;
                        lastSnapshot.SnapshotId = snapshot.SnapshotId;
                        lastSnapshot.Input = snapshot.Input;
                        _snapshots[lastSnapshotIndex] = lastSnapshot;
                        return;
                    }

                    lastSnapshot.Rotation = snapshot.Rotation;
                    lastSnapshot.ServerTime = snapshot.ServerTime;
                    lastSnapshot.SnapshotId = snapshot.SnapshotId;
                    lastSnapshot.Input = snapshot.Input;
                    _snapshots[lastSnapshotIndex] = lastSnapshot;
                    return;
                }
            }

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

        public long GetSnapshotId() => _snapshots.Count == 0 ? 0 : _snapshots[^1].SnapshotId;

        public long GetRenderSnapshotId()
        {
            switch (_snapshots.Count)
            {
                case 0:
                    return 0;
                case 1:
                    return _snapshots[0].SnapshotId;
                default:
                    GetInterpolationPair(out var older, out _, out _);
                    return older.SnapshotId;
            }
        }

        public void SyncServerTime(float serverTime)
        {
            if (serverTime <= 0f)
                return;

            var offset = Time.time - serverTime;

            if (!_hasServerTime)
            {
                _serverTimeOffset = offset;
                _hasServerTime = true;
                return;
            }
            
            _serverTimeOffset = Mathf.Lerp(_serverTimeOffset, offset, 0.1f);
        }
        
        private float GetServerTime()
        {
            if (!_hasServerTime)
                return 0f;
            
            return Time.time - _serverTimeOffset;
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
