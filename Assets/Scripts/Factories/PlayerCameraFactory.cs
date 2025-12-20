using System;
using Cysharp.Threading.Tasks;
using Db.Interface;
using Factories.Interface;
using Player.Camera;
using Services.Interface;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Factories
{
    public class PlayerCameraFactory : IInitializable, IPlayerCameraFactory, IDisposable
    {
        private readonly ILoaderService _loaderService;
        private readonly IGameData _gameData;
        private readonly IObjectResolver _resolver;

        private UniTask _initializeTask;
        private VirtualCameraView _cameraPrefab;

        public PlayerCameraFactory(
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
            var prefabRef = _gameData.PlayerCameraPrefab.Asset;

            _initializeTask = _loaderService.LoadResourcesUsingReference(prefabRef)
                .ContinueWith(cameraInstance =>
                {
                    if (!cameraInstance.resources.TryGetComponent(out VirtualCameraView view))
                        throw new Exception("Failed to load player camera");

                    _cameraPrefab = view;
                    return cameraInstance;
                });
        }

        public async UniTaskVoid CreateCamera(Transform target)
        {
            await UniTask.WaitUntil(() => _initializeTask.Status == UniTaskStatus.Succeeded);

            var cameraView = _resolver.Instantiate(_cameraPrefab);
            _resolver.Inject(cameraView);

            cameraView.InitializeCameras(target);
        }

        public void Dispose()
        {
            if (_cameraPrefab != null)
                _loaderService.ClearMemoryInstance(_cameraPrefab.gameObject);
        }
    }
}
