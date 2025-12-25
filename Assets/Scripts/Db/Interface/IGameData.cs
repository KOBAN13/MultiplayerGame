using Player.Camera;
using Player.Local;
using Player.Remote;
using Utils;

namespace Db.Interface
{
    public interface IGameData
    {
        AddressablePrefabByType<RemotePlayer> RemotePlayerPrefab { get; }
        AddressablePrefabByType<LocalPlayerMotor> LocalPlayerPrefab { get; }
        AddressablePrefabByType<VirtualCameraView> PlayerCameraPrefab { get; }
    }
}