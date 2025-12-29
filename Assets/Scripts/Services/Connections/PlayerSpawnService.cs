using System;
using Cysharp.Threading.Tasks;
using Factories.Interface;
using Player.Interface.Local;
using Player.Local;
using Player.Remote;
using Services.Db;
using Services.Interface;
using Sfs2X;
using UnityEngine;
using Utils.Enums;

namespace Services.Connections
{
    public class PlayerSpawnService : IPlayerSpawnService
    {
        private readonly IPlayerFactory _playerFactory;
        private readonly IPlayerCameraFactory _cameraFactory;
        private readonly IRemotePlayerRegistry _remotePlayerRegistry;
        private readonly SmartFox _sfs;

        public PlayerSpawnService(
            IPlayerFactory playerFactory,
            IPlayerCameraFactory cameraFactory,
            IRemotePlayerRegistry remotePlayerRegistry,
            SmartFox sfs
        )
        {
            _playerFactory = playerFactory;
            _cameraFactory = cameraFactory;
            _remotePlayerRegistry = remotePlayerRegistry;
            _sfs = sfs;
        }

        public async UniTask SpawnPlayer(PlayerJoinRequest joinRequest)
        {
            if (_remotePlayerRegistry.Contains(joinRequest.UserId))
            {
                Debug.LogWarning($"Player {joinRequest.UserId} already exists — skipping");
                return;
            }

            var playerType = joinRequest.PlayerType;

            switch (playerType)
            {
                case EPlayerType.Local:
                    var localPLayer = await _playerFactory.CreatePlayer<LocalPlayerMotor>(EPlayerType.Local, joinRequest.Position, Quaternion.identity);
                    InitializeLocalPlayer(joinRequest, localPLayer);
                    break;
                case EPlayerType.Remote:
                    var remotePlayer = await _playerFactory.CreatePlayer<RemotePlayer>(EPlayerType.Remote, joinRequest.Position, Quaternion.identity);
                    InitializeRemotePlayer(joinRequest, remotePlayer);
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(playerType), playerType, null);
            }
        }

        private void InitializeLocalPlayer(in PlayerJoinRequest joinRequest, LocalPlayerMotor player)
        {
            if (!_remotePlayerRegistry.TryAdd(joinRequest.UserId, player))
                return;

            player.SetSnapshot(joinRequest.Position, Vector3.zero, 0f, 0f);
            player.SetAnimationState(joinRequest.AnimationState);
            

            _cameraFactory.CreateCamera(player.GetCameraTarget()).Forget();
        }

        private void InitializeRemotePlayer(in PlayerJoinRequest joinRequest, RemotePlayer player)
        {
            if (!_remotePlayerRegistry.TryAdd(joinRequest.UserId, player))
                return;

            player.SetSnapshot(joinRequest.Position, Vector3.zero, 0f, 0f);
            player.SetAnimationState(joinRequest.AnimationState);
        }

        public void DeletePlayer(PlayerJoinRequest joinRequest)
        {
            
        }
    }
}
