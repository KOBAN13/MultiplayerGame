using Db.Interface;
using Factories;
using Input;
using Installer;
using Player.Camera;
using Player.Interface;
using Player.Interface.Local;
using Player.Local;
using Player.Prediction;
using Player.Remote;
using Player.Shared;
using Player.Weapon.Services;
using Services;
using Services.Connections;
using Services.Interface;
using Services.Prediction;
using Services.Snapshot;
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
            Register<InputFrameSender>(Lifetime.Singleton);
            Register<PredictionStateProvider>(Lifetime.Singleton);
            Register<InputFrameBuffer>(Lifetime.Singleton);
            Register<InputFrameSyncService>(Lifetime.Transient);
            Register<ReconciliationService>(Lifetime.Singleton);

            Builder.RegisterFactory<ISnapshotsServiceRegistry, int, IPlayerSnapshotReceiver>(
                (snapshotsServiceRegistry, playerId) => new PlayerSnapshotReceiver(snapshotsServiceRegistry, playerId));

            Builder.RegisterFactory<SnapshotCharacterMotorArgs, IPlayerSnapshotMotor>(
                args => new SnapshotCharacterMotor(args));
        }

        private void BindNetwork()
        {
            Register<NetworkStateReceiver>(Lifetime.Singleton);
            Register<ServerTimeService>(Lifetime.Singleton);
            Register<SnapshotsService>(Lifetime.Transient);
            Register<SnapshotsServiceRegistry>(Lifetime.Singleton);
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
            Register<PreconditionStorageService>(Lifetime.Singleton);
            Register<ShotFxSimulator>(Lifetime.Singleton);
        }
    }
}
