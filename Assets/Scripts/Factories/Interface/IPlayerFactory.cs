using Cysharp.Threading.Tasks;
using Player;
using Player.Interface;
using UnityEngine;

namespace Factories.Interface
{
    public interface IPlayerFactory
    {
        UniTask<IRemotePlayer> CreatePlayer(Vector3 position, Quaternion rotation);
    }
}