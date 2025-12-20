using Factories;
using Input;
using Installer;
using Player;
using Player.Camera;
using Services;
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