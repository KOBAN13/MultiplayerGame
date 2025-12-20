using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Factories.Interface;
using Helpers;
using Player.Interface;
using Services.Connections;
using Services.Db;
using Services.Interface;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using UnityEngine;
using VContainer.Unity;

namespace Player
{
    public class NetworkStateReceiver : IInitializable, IDisposable
    {
        private readonly SmartFox _sfs;
        private readonly IPlayerFactory _playerFactory;
        private readonly IPlayerCameraFactory _cameraFactory;
        private readonly Dictionary<int, IRemotePlayer> _remotePlayers;
        private readonly IPlayerJoinGameService _playerJoinGameService;
        
        private const string ROOM_GROUP_NAME = "Game";
        
        public NetworkStateReceiver(
            SmartFox sfs,
            IPlayerFactory playerFactory, 
            IPlayerCameraFactory cameraFactory
        )
        {
            _sfs = sfs;
            _playerFactory = playerFactory;
            _cameraFactory = cameraFactory;
            _remotePlayers = new Dictionary<int, IRemotePlayer>();
            _playerJoinGameService = new PlayerJoinGameService(this);
        }
        
        public void Initialize()
        {
            StartGame();
            
            _sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnPlayerState);
            _sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnPlayerEnterGame);
            
            _sfs.Send(new SubscribeRoomGroupRequest(ROOM_GROUP_NAME));
        }
        
        //TODO: Стоит добавить чтоб не приходили повторные id с сервера и убрать проверку id хоть и он приходит с сервера.
        public async UniTask InitializeRemotePlayer(PlayerJoinRequest joinRequest)
        {
            if (_remotePlayers.ContainsKey(joinRequest.UserId))
            {
                Debug.LogWarning($"Player {joinRequest.UserId} already exists — skipping");
                return;
            }

            var player = await _playerFactory.CreatePlayer(joinRequest.Position, Quaternion.identity);
            player.SetAnimationState(joinRequest.AnimationState);

            _remotePlayers.Add(joinRequest.UserId, player);

            Debug.Log($"Player {joinRequest.UserId} joined game");
            
            player.SetSnapshot(joinRequest.Position, Vector3.zero, 0f);
            player.SetAnimationState(joinRequest.AnimationState);

            if (_sfs.MySelf.Id == joinRequest.UserId)
            {
                _cameraFactory.CreateCamera(player.GetTransform()).Forget();
            }
        }
        
        public void Dispose()
        {
            _playerJoinGameService?.Dispose();
            
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

                _playerJoinGameService.AddPlayerJoinRequest(new PlayerJoinRequest(position, animationState, userId));
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
                var xRotationDirection = playerData.GetFloat("xRotationDirection");
                var zRotationDirection = playerData.GetFloat("zRotationDirection");
                var serverTime = playerData.GetFloat("serverTime");
                
                var animationState = playerData.GetUtfString("animationState");
                
                var position = new Vector3(xPosition, yPosition, zPosition);
                var rotationDirection = new Vector3(xRotationDirection, 0f, zRotationDirection);
                
                if (!_remotePlayers.TryGetValue(userId, out var remotePlayer))
                    continue;
                
                remotePlayer.SetSnapshot(position, rotationDirection, serverTime);
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