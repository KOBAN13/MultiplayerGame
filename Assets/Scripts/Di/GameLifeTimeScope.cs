using Db.Interface;
using Factories;
using Input;
using Installer;
using Player.Camera;
using Player.Interface;
using Player.Interface.Local;
using Player.Local;
using Player.Remote;
using Player.Shared;
using Player.Weapon.Projectile;
using Services;
using Services.Connections;
using Services.Interface;
using Sfs2X;
using UnityEngine;
using Utils.Pool;
using VContainer;

namespace Di
{
    public class GameLifeTimeScope : BaseLifeTimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            Builder = builder;
            BindService();
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
            Register<PlayerNetworkStateSender>(Lifetime.Singleton); 
            
            Builder.RegisterFactory<CharacterController,ClientStateProvider>(
                characterController => new ClientStateProvider(characterController));

            Builder.RegisterFactory<ISnapshotsService, CharacterController, Transform, IPlayerParameters, IPlayerSnapshotMotor>(
                (snapshotsService, characterController, playerTransform, playerParameters) =>
                    new SnapshotCharacterMotor(
                        snapshotsService,
                        characterController,
                        playerTransform, 
                        playerParameters));

            Builder.RegisterFactory<ISnapshotsService, IPlayerSnapshotReceiver>(
                snapshotsService => new PlayerSnapshotReceiver(snapshotsService));
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
        
        private void BindService()
        {
            Register<PoolService>(Lifetime.Singleton);
        }
    }
}
