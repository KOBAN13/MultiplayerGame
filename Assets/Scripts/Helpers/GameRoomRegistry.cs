using Db;
using ObservableCollections;

namespace Helpers
{
    public class GameRoomRegistry
    {
        public readonly ObservableDictionary<int, RoomData> Rooms = new();

        public void UpdateRoom(RoomData data)
        {
            Rooms[data.Id] = data;
        }

        public void RemoveRoom(int id)
        {
            Rooms.Remove(id);
        }

        public void Clear() => Rooms.Clear();
    }
}