using ObservableCollections;
using R3;
using Sfs2X.Entities;

namespace Services.Interface
{
    public interface ILobbyService
    {
        ObservableDictionary<int, User> Users { get; }
        Observable<Unit> JoinLobbySuccess { get; }
        Observable<string> JoinLobbyError { get; }
        Observable<Unit> KickedUser { get; }
        Observable<Unit> RoleChanged { get; }
        
        void KickUser(int userId);
        void JoinLobby(int roomId, string password = "");
        void StartGame();
        void LeaveLobby();
        void Initialize();
    }
}