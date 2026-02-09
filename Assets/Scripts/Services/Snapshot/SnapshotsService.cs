using System.Collections.Generic;
using Db.Interface;
using Player.Db;
using Services.Interface;
using UnityEngine;

namespace Services.Snapshot
{
    public class SnapshotsService : ISnapshotsService
    {
        private readonly List<SnapshotData> _snapshots = new();
        private readonly ISnapshotParameters _snapshotParameters;

        private readonly IServerTimeService _serverTimeService;
        private float _smoothedJitter;
        private float _lastArrivalTime;
        private float _lastServerTime;
        private bool _hasTiming;
        private float _lastDebugTime;

        public SnapshotsService(ISnapshotParameters snapshotParameters, IServerTimeService serverTimeService)
        {
            _snapshotParameters = snapshotParameters;
            _serverTimeService = serverTimeService;
        }

        public void AddSnapshot(in SnapshotData snapshot)
        {
            if (_snapshots.Count > 0 && snapshot.ServerTime <= _snapshots[^1].ServerTime)
                return;

            UpdateTiming(in snapshot);
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

        public (long snapshotId, byte alpha) GetRenderSnapshotId()
        {
            switch (_snapshots.Count)
            {
                case 0:
                    return (0, 0);
                case 1:
                    return (_snapshots[0].SnapshotId, 0);
                default:
                    GetInterpolationPair(out var older, out var newer, out var time);
                    var alpha = (byte) Mathf.Clamp(Mathf.RoundToInt(time * 255f), 0, 255);
                    return (older.SnapshotId, alpha);
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
            var backTime = GetInterpolationBackTime();
            var interpolationBackTime = serverTime - backTime;

            for (var i = _snapshots.Count - 1; i >= 0; i--)
            {
                if (_snapshots[i].ServerTime > interpolationBackTime && i != 0)
                    continue;

                older = _snapshots[i];
                newer = i < _snapshots.Count - 1 ? _snapshots[i + 1] : older;

                if (Mathf.Approximately(older.ServerTime, newer.ServerTime))
                    return;

                time = Mathf.InverseLerp(older.ServerTime, newer.ServerTime, interpolationBackTime);
                TryLogDebug(serverTime, backTime, older, newer, time);
                return;
            }

            older = _snapshots[^1];
            newer = older;
        }

        private void UpdateTiming(in SnapshotData snapshot)
        {
            var arrivalTime = Time.time;

            if (!_hasTiming)
            {
                _lastArrivalTime = arrivalTime;
                _lastServerTime = snapshot.ServerTime;
                _hasTiming = true;
                return;
            }

            var serverDelta = snapshot.ServerTime - _lastServerTime;
            var arrivalDelta = arrivalTime - _lastArrivalTime;

            if (serverDelta > 0f && arrivalDelta > 0f)
            {
                var diff = Mathf.Abs(arrivalDelta - serverDelta);
                var smoothing = GetJitterSmoothing();
                _smoothedJitter = Mathf.Lerp(_smoothedJitter, diff, smoothing);
            }

            _lastArrivalTime = arrivalTime;
            _lastServerTime = snapshot.ServerTime;
        }

        private float GetInterpolationBackTime()
        {
            var baseBackTime = _snapshotParameters.InterpolationBackTime;

            if (!_snapshotParameters.UseAdaptiveBackTime)
                return baseBackTime;

            var min = _snapshotParameters.AdaptiveBackTimeMin > 0f
                ? _snapshotParameters.AdaptiveBackTimeMin
                : baseBackTime;

            var max = _snapshotParameters.AdaptiveBackTimeMax > 0f
                ? _snapshotParameters.AdaptiveBackTimeMax
                : baseBackTime + 0.2f;

            var multiplier = _snapshotParameters.JitterMultiplier > 0f
                ? _snapshotParameters.JitterMultiplier
                : 2f;

            var adaptive = baseBackTime + _smoothedJitter * multiplier;
            return Mathf.Clamp(adaptive, min, max);
        }

        private float GetJitterSmoothing()
        {
            if (_snapshotParameters.JitterSmoothing > 0f && _snapshotParameters.JitterSmoothing <= 1f)
                return _snapshotParameters.JitterSmoothing;

            return 0.1f;
        }

        private void TryLogDebug(float serverTime, float backTime, SnapshotData older, SnapshotData newer, float t)
        {
            if (!_snapshotParameters.EnableInterpolationDebug)
                return;

            var interval = _snapshotParameters.DebugLogInterval > 0f ? _snapshotParameters.DebugLogInterval : 1f;
            if (Time.time - _lastDebugTime < interval)
                return;

            _lastDebugTime = Time.time;
            
            Debug.Log(
                $"[InterpolationDebug] count={_snapshots.Count} serverTime={serverTime:F3} backTime={backTime:F3} " +
                $"older={older.SnapshotId}:{older.ServerTime:F3} newer={newer.SnapshotId}:{newer.ServerTime:F3} " +
                $"t={t:F3} jitter={_smoothedJitter:F3}");
        }
    }
}
