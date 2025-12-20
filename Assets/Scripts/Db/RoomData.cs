using Sfs2X.Entities;

namespace Db
{
    public class RoomData
    {
        public readonly int Id;
        public readonly string Name;
        public readonly int UserCount;
        public readonly int MaxUsers;
        public readonly bool IsPrivate;

        public RoomData(Room room)
        {
            Id = room.Id;
            Name = room.Name;
            UserCount = room.UserCount;
            MaxUsers = room.MaxUsers;
            IsPrivate = room.IsPasswordProtected;
        }
    }
}