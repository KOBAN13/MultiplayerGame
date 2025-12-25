using Db.Interface;
using Player.Camera;
using Player.Local;
using Player.Remote;
using UnityEngine;
using Utils;

namespace Db
{
    [CreateAssetMenu(fileName = "GameData", menuName = "Db/GameData")]
    public class GameData : ScriptableObject, IGameData
    {
        [field: SerializeField] public AddressablePrefabByType<RemotePlayer> RemotePlayerPrefab { get; private set; }
        [field: SerializeField] public AddressablePrefabByType<LocalPlayerMotor> LocalPlayerPrefab { get; private set; }
        [field: SerializeField] public AddressablePrefabByType<VirtualCameraView> PlayerCameraPrefab { get; private set; }
    }
}