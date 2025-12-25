using Cysharp.Threading.Tasks;
using Services.Db;

namespace Services.Interface
{
    public interface IPlayerSpawnService
    {
        UniTask SpawnPlayer(PlayerJoinRequest joinRequest);
    }
}
