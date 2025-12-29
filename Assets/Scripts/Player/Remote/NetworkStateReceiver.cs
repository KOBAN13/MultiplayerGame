using System;
using Helpers;
using Services.Db;
using Services.Interface;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using UnityEngine;
using Utils.Enums;
using VContainer.Unity;

namespace Player.Remote
{
    public class NetworkStateReceiver : IInitializable, IDisposable
    {
        private readonly SmartFox _sfs;
        private readonly IPlayerJoinGameService _playerJoinGameService;
        private readonly IRemotePlayerRegistry _remotePlayerRegistry;
        
        private const string ROOM_GROUP_NAME = "Game";
        
        public NetworkStateReceiver(
            SmartFox sfs,
            IPlayerJoinGameService playerJoinGameService,
            IRemotePlayerRegistry remotePlayerRegistry
        )
        {
            _sfs = sfs;
            _playerJoinGameService = playerJoinGameService;
            _remotePlayerRegistry = remotePlayerRegistry;
        }
        
        public void Initialize()
        {
            StartGame();
            
            _sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnPlayerState);
            _sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnPlayerEnterGame);
            
            _sfs.Send(new SubscribeRoomGroupRequest(ROOM_GROUP_NAME));
        }
        
        public void Dispose()
        {
            _sfs.RemoveEventListener(SFSEvent.EXTENSION_RESPONSE, OnPlayerState);
            _sfs.RemoveEventListener(SFSEvent.EXTENSION_RESPONSE, OnPlayerEnterGame);
        }

        private void OnPlayerEnterGame(BaseEvent evt)
        {
            var cmd = (string)evt.Params[SFSResponseHelper.CMD];
            
            if (cmd != SFSResponseHelper.PLAYER_JOIN_ROOM)
                return;
            
            var data = (SFSObject)evt.Params["params"];
            var array = data.GetSFSArray("players");

            for (var i = 0; i < array.Count; i++)
            {
                var playerData = array.GetSFSObject(i);

                var userId = playerData.GetInt("userId");
                var x = playerData.GetFloat("x");
                var z = playerData.GetFloat("z");
                var animationState = playerData.GetUtfString("animationState");
                var position = new Vector3(x, 0f, z);
                
                var playerType = _sfs.MySelf.Id == userId 
                    ? EPlayerType.Local 
                    : EPlayerType.Remote;
                
                _playerJoinGameService.AddPlayerJoinRequest(new PlayerJoinRequest(position, animationState, userId, playerType));
            }
        }

        private void OnPlayerState(BaseEvent evt)
        {
            var cmd = (string)evt.Params[SFSResponseHelper.CMD];
            
            if (cmd != SFSResponseHelper.PLAYER_STATE)
                return;
            
            var data = (SFSObject)evt.Params["params"];
            var player = data.GetSFSArray("players");

            for (var i = 0; i < player.Count; i++)
            {
                var playerData = player.GetSFSObject(i);
                var userId = playerData.GetInt("userId");
                var xPosition = playerData.GetFloat("x");
                var yPosition = playerData.GetFloat("y");
                var zPosition = playerData.GetFloat("z");
                var serverTime = playerData.GetFloat("serverTime");
                var rotation = playerData.GetFloat("rotation");
                var xDirection = playerData.GetFloat("horizontal");
                var zDirection = playerData.GetFloat("vertical");
                
                var animationState = playerData.GetUtfString("animationState");
                
                var position = new Vector3(xPosition, yPosition, zPosition);
                var direction = new Vector3(xDirection, 0f, zDirection);
                
                if (!_remotePlayerRegistry.TryGet(userId, out var remotePlayer))
                    continue;
                
                remotePlayer.SetSnapshot(position, direction, rotation, serverTime);
                remotePlayer.SetAnimationState(animationState);
            }
        }
        
        private void StartGame()
        {
            var data = SFSObject.NewInstance();
            _sfs.Send(new ExtensionRequest(SFSResponseHelper.CREATE_GAME_ROOM, data, _sfs.LastJoinedRoom));
        }
    }
}
