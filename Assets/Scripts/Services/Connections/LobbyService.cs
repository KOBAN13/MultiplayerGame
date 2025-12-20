using System;
using Helpers;
using Manager.Interface;
using ObservableCollections;
using R3;
using Services.Interface;
using Services.SceneManagement;
using Services.SceneManagement.Enums;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using UnityEngine;
using Utils;

namespace Services.Connections
{
    public class LobbyService : ILobbyService, IDisposable
    {
        private readonly SmartFox _sfs;
        private readonly SceneLoader _sceneLoader;
        private readonly LobbyRegistry _registry;
        private readonly ReactiveProperty<string> _joinLobbyError = new();
        private readonly ISessionManager _sessionManager;
        private readonly Subject<Unit> _kickedUser = new();
        private readonly Subject<Unit> _joinLobbySuccess = new();
        private readonly Subject<Unit> _roleChanged = new();
            
        private const string ROOM_GROUP_NAME = "Game";
        
        public ObservableDictionary<int, User> Users => _registry.Users;
        public Observable<Unit> KickedUser => _kickedUser;
        public Observable<Unit> JoinLobbySuccess => _joinLobbySuccess;
        public Observable<string> JoinLobbyError => _joinLobbyError;
        public Observable<Unit> RoleChanged => _roleChanged;

        public LobbyService(
            SmartFox sfs, 
            LobbyRegistry registry, 
            ISessionManager sessionManager, 
            SceneLoader sceneLoader
        )
        {
            _sfs = sfs;
            _registry = registry;
            _sessionManager = sessionManager;
            _sceneLoader = sceneLoader;
        }

        public void KickUser(int userId)
        {
            if (_sessionManager.GetRole() != ERoomRole.OWNER)
                return;
            
            var data = SFSObject.NewInstance();
            data.PutInt("targetId", userId);
            
            _sfs.Send(new ExtensionRequest(SFSResponseHelper.KICK_USER_IN_ROOM, data, _sfs.LastJoinedRoom));
        }

        public void JoinLobby(int roomId, string password = "")
        {
            var data = SFSObject.NewInstance();
            data.PutInt("roomId", roomId);
            data.PutUtfString("roomPassword", password);
            
            _sfs.Send(new ExtensionRequest(SFSResponseHelper.USER_JOIN_ROOM, data));
        }
        
        public void StartGame()
        {
            var data = SFSObject.NewInstance();
            _sfs.Send(new ExtensionRequest(SFSResponseHelper.ROOM_START_GAME, data, _sfs.LastJoinedRoom));
        }

        public void LeaveLobby()
        {
            _sfs.Send(new LeaveRoomRequest());
        }

        public void Initialize()
        {
            _sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, HandleRoomJoinResponse);
            _sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, HandleKickUserResponse);
            _sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, HandleRoomUsersSync);
            _sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, HandleStartGameResponse);
            _sfs.AddEventListener(SFSEvent.ROOM_VARIABLES_UPDATE, HandleRoomVariablesChanged);
            _sfs.AddEventListener(SFSEvent.USER_ENTER_ROOM, HandleUserJoined);
            _sfs.AddEventListener(SFSEvent.ROOM_JOIN_ERROR, HandleRoomJoinError);
            _sfs.AddEventListener(SFSEvent.USER_EXIT_ROOM, HandleUserLeft);

            
            _sfs.Send(new SubscribeRoomGroupRequest(ROOM_GROUP_NAME));
        }

        private void HandleKickUserResponse(BaseEvent evt)
        {
            var cmd = (string)evt.Params[SFSResponseHelper.CMD];
            
            if (cmd != SFSResponseHelper.KICK_USER_IN_ROOM)
                return;
            
            var data = (ISFSObject)evt.Params["params"];
            var ok = data.GetBool(SFSResponseHelper.OK);
            
            if (ok)
            {
                _kickedUser.OnNext(Unit.Default);
            }
            else
            {
                Debug.LogError(data.GetUtfString(SFSResponseHelper.ERROR));
            }
        }
        
        private async void HandleStartGameResponse(BaseEvent evt)
        {
            var cmd = (string)evt.Params[SFSResponseHelper.CMD];
            
            if (cmd != SFSResponseHelper.ROOM_START_GAME)
                return;
            
            var data = (ISFSObject)evt.Params["params"];
            var ok = data.GetBool(SFSResponseHelper.OK);
            
            if (ok)
            {
                await _sceneLoader.LoadScene(TypeScene.Game);
            }
            else
            {
                Debug.LogError(data.GetUtfString(SFSResponseHelper.ERROR));
            }
        }

        private void HandleRoomUsersSync(BaseEvent evt)
        {
            var cmd = (string)evt.Params[SFSResponseHelper.CMD];

            if (cmd != SFSResponseHelper.ROOM_USER_CONNECTED)
                return;
            
            var data = (ISFSObject)evt.Params["params"];
            
            var roomId = data.GetInt("roomId");
            
            var room = _sfs.RoomManager.GetRoomById(roomId);

            foreach (var user in room.UserList)
            {
                var userId = user.Id;
                
                if (Users.ContainsKey(userId))
                    continue;
                
                HandleUser(user);
            }
        }

        private void HandleUserLeft(BaseEvent evt)
        {
            var user = (User) evt.Params["user"];
            
            _registry.RemoveUser(user.Id);
        }

        private void HandleUserJoined(BaseEvent evt)
        {
            var user = (User) evt.Params["user"];
            
            _registry.UpdateUser(user);
        }

        private void HandleRoomJoinResponse(BaseEvent evt)
        {
            var data = (ISFSObject)evt.Params["params"];
            var cmd = (string)evt.Params[SFSResponseHelper.CMD];

            if (cmd != SFSResponseHelper.USER_JOIN_ROOM)
                return;

            var result = data.GetBool(SFSResponseHelper.OK);

            if (result)
            {
                var roleName = data.GetUtfString("role");
                var userId = data.GetInt("userId");

                if (!Enum.TryParse(roleName, out ERoomRole parsed)) 
                    return;
                
                
                //TODO: Супер колхоз нет единой системы для ролей, не должно лобби этим заниматся придумать и вынести
                var user = _sfs.UserManager.GetUserById(userId);
                _sessionManager.SetRole(parsed);
                HandleUser(user);
                _joinLobbySuccess.OnNext(Unit.Default);
                _roleChanged.OnNext(Unit.Default);
            }
            else
            {
                _joinLobbyError.Value = data.GetUtfString(SFSResponseHelper.ERROR);
            }
        }

        private void HandleRoomVariablesChanged(BaseEvent evt)
        {
            
        }

        private void HandleRoomJoinError(BaseEvent evt)
        {
            var errorMessage = "Room join failed: " + (string)evt.Params["errorMessage"];
        }

        private void HandleUser(User user)
        {
            _registry.UpdateUser(user);
        }

        public void Dispose()
        {
            _joinLobbyError?.Dispose();
            _kickedUser?.Dispose();
            _joinLobbySuccess?.Dispose();
            _roleChanged?.Dispose();
            
            _registry.Clear();
            
            _sfs.RemoveEventListener(SFSEvent.EXTENSION_RESPONSE, HandleRoomJoinResponse);
            _sfs.RemoveEventListener(SFSEvent.EXTENSION_RESPONSE, HandleRoomUsersSync);
            _sfs.RemoveEventListener(SFSEvent.EXTENSION_RESPONSE, HandleKickUserResponse);
            _sfs.RemoveEventListener(SFSEvent.EXTENSION_RESPONSE, HandleStartGameResponse);
            _sfs.RemoveEventListener(SFSEvent.ROOM_VARIABLES_UPDATE, HandleRoomVariablesChanged);
            _sfs.RemoveEventListener(SFSEvent.USER_ENTER_ROOM, HandleUserJoined);
            _sfs.RemoveEventListener(SFSEvent.ROOM_JOIN_ERROR, HandleRoomJoinError);
            _sfs.RemoveEventListener(SFSEvent.USER_EXIT_ROOM, HandleUserLeft);
        }
    }
}