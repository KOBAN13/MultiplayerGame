using Player;
using Player.Camera;
using Player.Remote;
using Utils;

namespace Db.Interface
{
    public interface IGameData
    {
        AddressablePrefabByType<RemotePlayer> PlayerPrefab { get; }
        AddressablePrefabByType<VirtualCameraView> PlayerCameraPrefab { get; }
    }
}