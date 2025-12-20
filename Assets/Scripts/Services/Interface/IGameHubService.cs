using Db;
using ObservableCollections;
using R3;

namespace Services.Interface
{
    public interface IGameHubService
    {
        Observable<string> CreateRoomError { get; }
        Observable<Unit> CreateRoomCommand { get; }
        ObservableDictionary<int, RoomData> Rooms { get; }
        
        void Initialize();
        void CreateRoom(string name, short maxUsers, bool isPrivate, string password);
    }
}