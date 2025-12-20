using System;
using System.Linq;
using Manager.Interface;
using R3;
using Sfs2X;
using Sfs2X.Entities;
using Utils;

namespace Manager
{
    public class SessionManager : ISessionManager
    {
        private readonly SmartFox _sfs;
        private User User => _sfs.MySelf;
        private Room CurrentRoom => _sfs.LastJoinedRoom;
        
        private readonly ReactiveProperty<ERoomRole> _myRole = new();
        
        public Observable<ERoomRole> MyRole => _myRole;

        public SessionManager(SmartFox sfs)
        {
            _sfs = sfs ?? throw new ArgumentNullException(nameof(sfs));
        }

        public T RoomVariables<T>(string keyVariable) 
        {
            if (CurrentRoom == null)
                throw new InvalidOperationException("No room is currently set.");
            
            var variables = CurrentRoom.GetVariable(keyVariable);
            return (T)variables.Value;
        }
        
        public T UserVariables<T>(string keyVariable)
        {
            if (User == null)
                throw new InvalidOperationException("User is not initialized.");
            
            var variables = User.GetVariable(keyVariable);
            return (T)variables.Value;
        }
        
        public void SetRole(ERoomRole role)
        {
            _myRole.Value = role;
        }
        
        public ERoomRole GetRole()
        {
            return _myRole.Value;
        }
        
        public int FindUserIdByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty.", nameof(name));

            var room = _sfs.LastJoinedRoom ?? CurrentRoom;

            if (room == null)
                throw new InvalidOperationException("User is not in a room.");

            var user = room.UserList.FirstOrDefault(u => u.Name == name);
            
            return user?.Id ?? -1;
        }
    }
}