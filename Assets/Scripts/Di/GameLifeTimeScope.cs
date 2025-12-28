using Db.Interface;
using Factories;
using Input;
using Installer;
using Player;
using Player.Camera;
using Player.Interface;
using Player.Interface.Local;
using Player.Local;
using Player.Remote;
using Player.Shared;
using Player.Shoot;
using Services;
using Services.Connections;
using Services.Interface;
using Sfs2X;
using UnityEngine;
using VContainer;

namespace Di
{
    public class GameLifeTimeScope : BaseLifeTimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            Builder = builder;
            BindNetwork();
            BindInput();
            BindInstaller();
            BindPlayer();
        }

        private void BindPlayer()
        {
            Register<PlayerFactory>(Lifetime.Singleton);
            Register<PlayerCameraFactory>(Lifetime.Singleton);
            Register<PlayerCameraHolder>(Lifetime.Singleton);
            Register<RemotePlayerRegistry>(Lifetime.Singleton);
            Register<PlayerSpawnService>(Lifetime.Singleton);
            Register<PlayerJoinGameService>(Lifetime.Singleton);
            Register<SimpleShotController>(Lifetime.Scoped);

            Builder.RegisterFactory<ISnapshotsService, CharacterController, Transform, IPlayerParameters, IPlayerSnapshotMotor>(
                (snapshotsService, characterController, playerTransform, playerParameters) =>
                    new SnapshotCharacterMotor(
                        snapshotsService,
                        characterController,
                        playerTransform, 
                        playerParameters));

            Builder.RegisterFactory<ISnapshotsService, IPlayerSnapshotReceiver>(
                snapshotsService => new PlayerSnapshotReceiver(snapshotsService));

            Builder.RegisterFactory<SmartFox, CharacterController, Transform, IPlayerNetworkInputSender>(
                (sfs, characterController, cameraTarget) =>
                    new PlayerNetworkInputSender(sfs, characterController, cameraTarget));
        }

        private void BindNetwork()
        {
            Register<NetworkStateReceiver>(Lifetime.Singleton);
            Register<SnapshotsService>(Lifetime.Transient);
        }

        private void BindInstaller()
        {
            Register<GameInstaller>(Lifetime.Singleton);
        }

        private void BindInput()
        {
            Register<PlayerInput>(Lifetime.Singleton);
            Register<PlayerNetworkInputReader>(Lifetime.Singleton);
            Register<LocalInputSource>(Lifetime.Singleton);
        }
    }
}
