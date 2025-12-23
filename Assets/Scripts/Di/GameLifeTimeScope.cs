using Db.Interface;
using Factories;
using Input;
using Installer;
using Player;
using Player.Camera;
using Player.Interface;
using Services;
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

            Builder.RegisterFactory<ISnapshotsService, CharacterController, Transform, float, IRemotePlayerMovement>(
                (snapshotsService, characterController, playerTransform, smoothSpeed) =>
                    new RemotePlayerMovement(snapshotsService, characterController, playerTransform, smoothSpeed));

            Builder.RegisterFactory<ISnapshotsService, CharacterController, Transform, float, IRotationComponent>(
                resolver =>
                    (snapshotsService, characterController, playerTransform, rotationSpeed) =>
                        new RemoteRotationPlayer(
                            characterController,
                            playerTransform,
                            rotationSpeed,
                            snapshotsService,
                            resolver.Resolve<IRotationCameraParameters>()),
                Lifetime.Singleton);

            Builder.RegisterFactory<ISnapshotsService, IPlayerSnapshotReceiver>(
                snapshotsService => new PlayerSnapshotReceiver(snapshotsService));

            Builder.RegisterFactory<IPlayerNetworkInputReader, SmartFox, CharacterController, Transform, IPlayerNetworkInputSender>(
                (playerNetworkInputReader, sfs, characterController, cameraTarget) =>
                    new PlayerNetworkInputSender(playerNetworkInputReader, sfs, characterController, cameraTarget));
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
        }
    }
}
