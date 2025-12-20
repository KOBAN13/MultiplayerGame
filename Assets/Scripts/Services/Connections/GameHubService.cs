using System;
using System.Collections.Generic;
using Db;
using Helpers;
using Manager.Interface;
using ObservableCollections;
using R3;
using Services.Interface;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using UnityEngine;
using Utils;

namespace Services.Connections
{
    public class GameHubService : IGameHubService, IDisposable
    {
        private readonly SmartFox _sfs;
        private readonly GameRoomRegistry _registry;
        private readonly ReactiveProperty<string> _createRoomError = new();
        private readonly ReactiveCommand<Unit> _createRoomCommand = new();
        private readonly ISessionManager _sessionManager;

        private const string GAME_ROOMS_GROUP_NAME = "Game";

        public Observable<string> CreateRoomError => _createRoomError;
        public Observable<Unit> CreateRoomCommand => _createRoomCommand;
        public ObservableDictionary<int, RoomData> Rooms => _registry.Rooms;

        public GameHubService(SmartFox sfs, GameRoomRegistry registry, ISessionManager sessionManager)
        {
            _sfs = sfs;
            _registry = registry;
            _sessionManager = sessionManager;
        }

        public void Initialize()
        {
            _sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnRoomCreationResult);
            _sfs.AddEventListener(SFSEvent.ROOM_ADD, OnRoomAdded);
            _sfs.AddEventListener(SFSEvent.ROOM_REMOVE, OnRoomRemoved);
            _sfs.AddEventListener(SFSEvent.USER_COUNT_CHANGE, OnUserCountChanged);
            _sfs.AddEventListener(SFSEvent.ROOM_GROUP_SUBSCRIBE, OnRoomGroupSubscribe);

            _sfs.Send(new SubscribeRoomGroupRequest(GAME_ROOMS_GROUP_NAME));
        }

        public void CreateRoom(string name, short maxUsers, bool isPrivate, string password)
        {
            var sfsObject = SFSObject.NewInstance();
            sfsObject.PutInt("ownerId", _sfs.MySelf.Id);
            sfsObject.PutUtfString("roomName", name);
            sfsObject.PutShort("maxUsers", maxUsers);
            sfsObject.PutUtfString("roomPassword", password);
            
            _sfs.Send(new ExtensionRequest(SFSResponseHelper.CREATE_ROOM, sfsObject));
        }

        private void OnRoomGroupSubscribe(BaseEvent evt)
        {
            var rooms = (List<Room>)evt.Params["newRooms"];
            
            foreach (var room in rooms)
                HandleRoom(room);
        }

        private void OnUserCountChanged(BaseEvent evt)
            => HandleRoom((Room)evt.Params["room"]);
        
        private void OnRoomAdded(BaseEvent evt)
        {
            var room = (Room)evt.Params["room"];
            
            HandleRoom(room);
        }

        private void OnRoomRemoved(BaseEvent evt)
        {
            var room = (Room)evt.Params["room"];
            _registry.RemoveRoom(room.Id);
        }

        private void OnRoomCreationResult(BaseEvent evt)
        {
            var data = (ISFSObject) evt.Params["params"];
            var cmd = (string)evt.Params[SFSResponseHelper.CMD];

            if (cmd != SFSResponseHelper.CREATE_ROOM) 
                return;
            
            var result = data.GetBool(SFSResponseHelper.OK);

            if (result)
            {
                var roleName = data.GetUtfString("role");
                
                if (Enum.TryParse(roleName, out ERoomRole parsed))
                {
                    _sessionManager.SetRole(parsed);
                }
                
                _createRoomCommand.Execute(Unit.Default);
            }
            else
            {
                _createRoomError.Value = data.GetUtfString(SFSResponseHelper.ERROR);
            }
        }

        private void HandleRoom(Room room)
        {
            if (room.IsHidden)
                return;
            
            _registry.UpdateRoom(new RoomData(room));
        }

        public void Dispose()
        {
            _createRoomError?.Dispose();
            _createRoomCommand?.Dispose();
            _registry.Clear();
            
            _sfs.RemoveEventListener(SFSEvent.EXTENSION_RESPONSE, OnRoomCreationResult);
            _sfs.RemoveEventListener(SFSEvent.ROOM_ADD, OnRoomAdded);
            _sfs.RemoveEventListener(SFSEvent.ROOM_REMOVE, OnRoomRemoved);
            _sfs.RemoveEventListener(SFSEvent.USER_COUNT_CHANGE, OnUserCountChanged);
            _sfs.RemoveEventListener(SFSEvent.ROOM_GROUP_SUBSCRIBE, OnRoomGroupSubscribe);
        }
    }
}
