using System.Linq;
using Factories;
using Manager;
using Player;
using Services;
using Services.Connections;
using Services.Helpers;
using Services.SceneManagement;
using Sfs2X;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;

namespace Di
{
    public class RootLifeTimeScope : BaseLifeTimeScope
    {
        [SerializeField] private SceneLoader _sceneLoader;
        [SerializeField] private AssetLabelReference _configLable;
        
        protected override void Configure(IContainerBuilder builder)
        {
            Builder = builder;
            
            RegisterConfigs();
            RegisterFactories();
            RegisterServices();
        }

        private void RegisterConfigs()
        {
            var configs = Addressables
                .LoadAssetsAsync<ScriptableObject>(_configLable, null)
                .WaitForCompletion()
                .ToList();

            foreach (var config in configs)
            {
                RegisterInstance(config);
            }
        }

        private void RegisterFactories()
        {
            Register<ViewModelFactory>(Lifetime.Singleton);
            Register<ViewsFactory>(Lifetime.Singleton);
            Register<ScreensFactory>(Lifetime.Singleton);
        }

        private void RegisterServices()
        {
            var sfs = new SmartFox();
            
            RegisterInstance(sfs);
            Register<LoaderService>(Lifetime.Singleton);
            Register<SceneResources>(Lifetime.Singleton);
            Register<SceneService>(Lifetime.Singleton);
            RegisterInstance(_sceneLoader);
            Register<ScreenService>(Lifetime.Singleton);
            RegisterWithArgument<EncryptionService, string>(Lifetime.Singleton, "SFSTestGameKey568");
            Register<ConnectionService>(Lifetime.Singleton);
            Register<LoginClientService>(Lifetime.Singleton);
            Register<SessionManager>(Lifetime.Singleton);
        }
    }
}