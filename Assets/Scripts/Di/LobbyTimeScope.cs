using Factories;
using Helpers;
using Installer;
using Pool;
using Services.Connections;
using UnityEngine;
using VContainer;

namespace Di
{
    public class LobbyTimeScope : BaseLifeTimeScope
    {
        [SerializeField] private LobbyInstaller _lobbyInstaller;
        
        protected override void Configure(IContainerBuilder builder)
        {
            Builder = builder;
            
            var root = Find<RootLifeTimeScope>();
            EnqueueParent(root);
            
            RegisterFactories();
            RegisterServices();
            ServerScope();
        }
        
        private void RegisterFactories()
        {
            Register<ViewModelFactory>(Lifetime.Singleton);
            Register<ViewsFactory>(Lifetime.Singleton);
            Register<ScreensFactory>(Lifetime.Singleton);
        }

        private void RegisterServices()
        {
            Register<PingService>(Lifetime.Singleton);
            Register<GameRoomRegistry>(Lifetime.Singleton);
            Register<LobbyRegistry>(Lifetime.Singleton);
            Register<GameListItemPool>(Lifetime.Singleton);
            Register<PlayerLobbyItemPool>(Lifetime.Singleton);
            Register<GameHubService>(Lifetime.Singleton);
            Register<LobbyService>(Lifetime.Singleton);
        }
        

        private void ServerScope()
        {
            RegisterInstance(_lobbyInstaller);
        }
    }
}