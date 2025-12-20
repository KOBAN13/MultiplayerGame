using Services.Db;

namespace Services.Interface
{
    public interface IPlayerJoinGameService
    {
        void AddPlayerJoinRequest(PlayerJoinRequest joinRequest);
        void Dispose();
    }
}