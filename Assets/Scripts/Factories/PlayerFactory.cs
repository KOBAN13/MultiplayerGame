using System;
using Cysharp.Threading.Tasks;
using Db;
using Db.Interface;
using Factories.Interface;
using Player;
using Player.Interface;
using Player.Local;
using Player.Remote;
using Player.Utils;
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

        private UniTask _loadTask;
        private RemotePlayer _remotePlayerPrefab;
        private LocalPlayerMotor _localPlayerMotorPrefab;

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
            var remotePrefabRef = _gameData.RemotePlayerPrefab.Asset;
            var localPrefabRef = _gameData.LocalPlayerPrefab.Asset;
        
            var loadRemotePrefab = _loaderService.
                LoadResourcesUsingReference(remotePrefabRef)
                .ContinueWith(playerInstance =>
            {
                if (!playerInstance.resources.TryGetComponent(out RemotePlayer view))
                    throw new Exception("Failed to load player");

                _remotePlayerPrefab = view;
                return playerInstance;
            });

            var loadLocalPrefab = _loaderService
                .LoadResourcesUsingReference(localPrefabRef)
                .ContinueWith(playerInstance =>
            {
                if (!playerInstance.resources.TryGetComponent(out LocalPlayerMotor view))
                    throw new Exception("Failed to load player");

                _localPlayerMotorPrefab = view;
                return playerInstance;
            });
            
            _loadTask = UniTask.WhenAll(loadRemotePrefab, loadLocalPrefab);
        }
    
        public async UniTask<T> CreatePlayer<T>(EPlayerType playerType, Vector3 position, Quaternion rotation)
            where T : Component
        {
            await _loadTask;

            return playerType switch
            {
                EPlayerType.Local  => PlayerView((T)(Component)_localPlayerMotorPrefab, position, rotation),
                EPlayerType.Remote => PlayerView((T)(Component)_remotePlayerPrefab, position, rotation),
                _ => throw new ArgumentOutOfRangeException(nameof(playerType), playerType, null)
            };
        }

        private T PlayerView<T>(T prefab, Vector3 position, Quaternion rotation) where T : Component
        {
            var playerView = _resolver.Instantiate(prefab);
            playerView.transform.SetPositionAndRotation(position, rotation);
            _resolver.Inject(playerView);

            return playerView;
        }

        public void Dispose()
        {
            if (_remotePlayerPrefab != null)
                _loaderService.ClearMemoryInstance(_remotePlayerPrefab.gameObject);
            
            if (_localPlayerMotorPrefab != null)
                _loaderService.ClearMemoryInstance(_localPlayerMotorPrefab.gameObject);
        }
    }
}