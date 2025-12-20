using Db.Interface;
using Player;
using Player.Camera;
using UnityEngine;
using Utils;

namespace Db
{
    [CreateAssetMenu(fileName = "GameData", menuName = "Db/GameData")]
    public class GameData : ScriptableObject, IGameData
    {
        [field: SerializeField] public AddressablePrefabByType<RemotePlayer> PlayerPrefab { get; private set; }
        [field: SerializeField] public AddressablePrefabByType<VirtualCameraView> PlayerCameraPrefab { get; private set; }
    }
}