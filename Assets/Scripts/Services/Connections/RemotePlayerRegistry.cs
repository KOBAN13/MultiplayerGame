using System.Collections.Generic;
using Player;
using Player.Interface;
using Services.Interface;

namespace Services.Connections
{
    public class RemotePlayerRegistry : IRemotePlayerRegistry
    {
        private readonly Dictionary<int, APlayer> _remotePlayers = new();

        public bool Contains(int userId)
        {
            return _remotePlayers.ContainsKey(userId);
        }

        public bool TryAdd(int userId, APlayer remotePlayer)
        {
            return _remotePlayers.TryAdd(userId, remotePlayer);
        }

        public bool TryGet(int userId, out APlayer remotePlayer)
        {
            return _remotePlayers.TryGetValue(userId, out remotePlayer);
        }

        public bool Remove(int userId)
        {
            return _remotePlayers.Remove(userId);
        }

        public void Clear()
        {
            _remotePlayers.Clear();
        }
    }
}
