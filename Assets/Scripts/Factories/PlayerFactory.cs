using System;
using Cysharp.Threading.Tasks;
using Db;
using Db.Interface;
using Factories.Interface;
using Player;
using Player.Interface;
using Services.Interface;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Factories
{
    public class PlayerFactory : IInitializable, IPlayerFactory, IDisposable
    {
        private readonly ILoaderService _loaderService;
        private readonly IGameData _gameData;
        private readonly IObjectResolver _resolver;

        private UniTask _initializeTask;
        private RemotePlayer _playerPrefab;

        public PlayerFactory(
            ILoaderService loaderService,
            IGameData gameData,
            IObjectResolver resolver)
        {
            _loaderService = loaderService;
            _gameData = gameData;
            _resolver = resolver;
        }
    
        public void Initialize()
        {
            var prefabRef = _gameData.PlayerPrefab.Asset;
        
            _initializeTask = _loaderService.LoadResourcesUsingReference(prefabRef)
                .ContinueWith(playerInstance =>
            {
                if (!playerInstance.resources.TryGetComponent(out RemotePlayer view))
                    throw new Exception("Failed to load player");

                _playerPrefab = view;
                return playerInstance;
            });
        }
    
        public async UniTask<IRemotePlayer> CreatePlayer(Vector3 position, Quaternion rotation)
        {
            await UniTask.WaitUntil(() => _initializeTask.Status == UniTaskStatus.Succeeded);
            
            var playerView = _resolver.Instantiate(_playerPrefab);
            playerView.transform.SetPositionAndRotation(position, rotation);
            _resolver.Inject(playerView);

            return playerView;
        }

        public void Dispose()
        {
            if (_playerPrefab != null)
                _loaderService.ClearMemoryInstance(_playerPrefab.gameObject);
        }
    }
}