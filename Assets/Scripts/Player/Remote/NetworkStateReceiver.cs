using System;
using Db.Projectile;
using Helpers;
using Player.Db;
using Player.Weapon;
using Player.Weapon.Data;
using Player.Weapon.Services;
using Services.Db;
using Services.Interface;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using UnityEngine;
using Utils.Enums;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace Player.Remote
{
    public class NetworkStateReceiver : IInitializable, IDisposable
    {
        private readonly SmartFox _sfs;
        private readonly IPlayerJoinGameService _playerJoinGameService;
        private readonly IRemotePlayerRegistry _remotePlayerRegistry;
        private readonly IReconciliationService _reconciliationService;
        private readonly ISnapshotsServiceRegistry _snapshotsServiceRegistry;
        private readonly IShotFxSimulator _shotFxSimulator;
        private readonly AHitScanWeaponData _defaultHitScanWeaponData;
        
        private const string ROOM_GROUP_NAME = "Game";
        
        public NetworkStateReceiver(
            SmartFox sfs,
            IPlayerJoinGameService playerJoinGameService,
            IRemotePlayerRegistry remotePlayerRegistry, 
            IReconciliationService reconciliationService, 
            ISnapshotsServiceRegistry snapshotsServiceRegistry,
            IShotFxSimulator shotFxSimulator,
            WeaponData weaponData)
        {
            _sfs = sfs;
            _playerJoinGameService = playerJoinGameService;
            _remotePlayerRegistry = remotePlayerRegistry;
            _reconciliationService = reconciliationService;
            _snapshotsServiceRegistry = snapshotsServiceRegistry;
            _shotFxSimulator = shotFxSimulator;

            if (weaponData != null
                && weaponData.TryGet(EWeaponType.SingleShot, out var singleShotData)
                && singleShotData is AHitScanWeaponData hitScanWeaponData)
            {
                _defaultHitScanWeaponData = hitScanWeaponData;
            }
        }
        
        public void Initialize()
        {
            StartGame();
            
            _sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnServerPlayerState);
            _sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnPlayerEnterGame);
            _sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnPlayerLeaveGame);
            _sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnRemoteClientShoot);
            
            _sfs.Send(new SubscribeRoomGroupRequest(ROOM_GROUP_NAME));
        }
        
        private void OnRemoteClientShoot(BaseEvent evt)
        {
            var cmd = (string)evt.Params[SFSResponseHelper.CMD];
            
            if (cmd != SFSResponseHelper.RAYCAST_EXCLUDE_SENDER)
                return;

            var data = (SFSObject)evt.Params["params"];

            var hitX = data.GetFloat("xPoint");
            var hitY = data.GetFloat("yPoint");
            var hitZ = data.GetFloat("zPoint");
            var isHit = data.GetBool("hit");

            var shotData = new ClientFireCommand
            {
                hitPosition = new Vector3(hitX, hitY, hitZ),
                isHit = isHit
            };

            _shotFxSimulator.SimulateShotServer(_defaultHitScanWeaponData, shotData);
        }

        private void OnPlayerLeaveGame(BaseEvent evt)
        {
            var cmd = (string)evt.Params[SFSResponseHelper.CMD];
            
            if (cmd != SFSResponseHelper.PLAYER_LEAVE_GAME_ROOM)
                return;
            
            var data = (SFSObject)evt.Params["params"];
            var userId = data.GetInt(SFSResponseHelper.USER_ID);

            if (_remotePlayerRegistry.TryGet(userId, out var player))
            {
                _remotePlayerRegistry.Remove(userId);
                Object.Destroy(player.gameObject);
            }
            else
            {
                _remotePlayerRegistry.Remove(userId);
            }
            
            _snapshotsServiceRegistry.RemoveSnapshotService(userId);
        }

        private void OnPlayerEnterGame(BaseEvent evt)
        {
            var cmd = (string)evt.Params[SFSResponseHelper.CMD];
            
            if (cmd != SFSResponseHelper.PLAYER_JOIN_GAME_ROOM)
                return;
            
            var data = (SFSObject)evt.Params["params"];
            var array = data.GetSFSArray("players");

            for (var i = 0; i < array.Count; i++)
            {
                var playerData = array.GetSFSObject(i);

                var userId = playerData.GetInt("userId");
                var x = playerData.GetFloat("x");
                var z = playerData.GetFloat("z");
                var animationState = playerData.GetInt("animationState");
                var position = new Vector3(x, 0f, z);
                
                var playerType = _sfs.MySelf.Id == userId 
                    ? EPlayerType.Local 
                    : EPlayerType.Remote;

                _playerJoinGameService.AddPlayerJoinRequest(new PlayerJoinRequest(position, animationState, userId, playerType));
            }
        }

        private void OnServerPlayerState(BaseEvent evt)
        {
            var cmd = (string)evt.Params[SFSResponseHelper.CMD];
            
            if (cmd != SFSResponseHelper.PLAYER_SERVER_STATE)
                return;
            
            var data = (SFSObject)evt.Params["params"];
            var player = data.GetSFSArray("players");

            for (var i = 0; i < player.Count; i++)
            {
                var playerData = player.GetSFSObject(i);
                var userId = playerData.GetInt("userId");
                var snapshotId = playerData.GetLong("snapshotId");
                var lastProcessedInputSequence = playerData.GetLong("lastProcessedInputSequence");
                var xPosition = playerData.GetFloat("x");
                var yPosition = playerData.GetFloat("y");
                var zPosition = playerData.GetFloat("z");
                var serverTime = playerData.GetFloat("serverTime");
                var rotation = playerData.GetFloat("rotation");
                var xDirection = playerData.GetFloat("horizontal");
                var zDirection = playerData.GetFloat("vertical");
                var isGrounded = playerData.GetBool("isOnGround");
                var isAim = playerData.GetBool("isAim");
                var aimDirectionX = playerData.GetFloat("aimDirectionX");
                var aimDirectionY = playerData.GetFloat("aimDirectionY");
                var aimDirectionZ = playerData.GetFloat("aimDirectionZ");
                var aimPitch = playerData.GetFloat("aimPitch");
                
                var animationState = playerData.GetInt("animationState");
                
                var position = new Vector3(xPosition, yPosition, zPosition);
                var direction = new Vector3(xDirection, 0f, zDirection);
                
                if (!_remotePlayerRegistry.TryGet(userId, out var remotePlayer))
                    continue;
                
                var snapshotData = new SnapshotData()
                {
                    AimData = new AimData
                    {
                        AimDirection = new Vector3(aimDirectionX, aimDirectionY, aimDirectionZ),
                        AimPitch = aimPitch,
                        isAim = isAim
                    },
                    
                    Position = position,
                    Input = direction,
                    Rotation = rotation,
                    IsGrounded = isGrounded,
                    AnimationState = animationState,
                    ServerTime = serverTime,
                    SnapshotId = snapshotId,
                    LastProcessedInputSequence = lastProcessedInputSequence
                };
                
                remotePlayer.SetSnapshot(in snapshotData);
                remotePlayer.SetAnimationState(animationState);
                
                _reconciliationService.Reconciliation(remotePlayer, snapshotData.Position, snapshotData);
            }
        }
        
        private void StartGame()
        {
            var data = SFSObject.NewInstance();
            _sfs.Send(new ExtensionRequest(SFSResponseHelper.CREATE_GAME_ROOM, data, _sfs.LastJoinedRoom));
        }
        
        public void Dispose()
        {
            _sfs.RemoveEventListener(SFSEvent.EXTENSION_RESPONSE, OnServerPlayerState);
            _sfs.RemoveEventListener(SFSEvent.EXTENSION_RESPONSE, OnPlayerEnterGame);
            _sfs.RemoveEventListener(SFSEvent.EXTENSION_RESPONSE, OnPlayerLeaveGame);
            _sfs.RemoveEventListener(SFSEvent.EXTENSION_RESPONSE, OnRemoteClientShoot);
        }
    }
}
