using Player.Interface;

namespace Services.Interface
{
    public interface IRemotePlayerRegistry
    {
        bool Contains(int userId);
        bool TryAdd(int userId, IRemotePlayer remotePlayer);
        bool TryGet(int userId, out IRemotePlayer remotePlayer);
        bool Remove(int userId);
        void Clear();
    }
}
