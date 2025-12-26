using Player;

namespace Services.Interface
{
    public interface IRemotePlayerRegistry
    {
        bool Contains(int userId);
        bool TryAdd(int userId, APlayer remotePlayer);
        bool TryGet(int userId, out APlayer remotePlayer);
        bool Remove(int userId);
        void Clear();
    }
}
