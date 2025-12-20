using Factories;
using Installer;
using Services;
using Services.Connections;
using UnityEngine;
using VContainer;

namespace Di
{
    public class MainMenuTimeScope : BaseLifeTimeScope
    {
        [SerializeField] private MainMenuInstaller _mainMenuInstaller;
        
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
            Register<ScreenService>(Lifetime.Singleton);
            Register<NoteService>(Lifetime.Singleton);
        }
        
        private void ServerScope()
        {
            RegisterInstance(_mainMenuInstaller);
            Register<RegistrationService>(Lifetime.Singleton);
            Register<RestorePasswordService>(Lifetime.Singleton);
        }
    }
}