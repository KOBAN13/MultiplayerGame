using Services.Interface;
using UnityEngine;

namespace Services.Snapshot
{
    public class ServerTimeService : IServerTimeService
    {
        private bool _hasServerTime;
        private float _serverTimeOffset;

        public bool HasServerTime => _hasServerTime;

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

        public float GetServerTime()
        {
            if (!_hasServerTime)
                return 0f;

            return Time.time - _serverTimeOffset;
        }
    }
}
