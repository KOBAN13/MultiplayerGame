using System;
using Cysharp.Threading.Tasks;
using Eflatun.SceneReference;
using Services.Interface;
using Services.SceneManagement.Enums;
using R3;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Services.SceneManagement
{
    public class SceneService : IScenesService
    {
        private SceneResources _resources;
        private SceneLoader _sceneLoader;
        private AsyncOperationHandle<SceneInstance> _currentSceneHandle;
        private readonly Subject<Unit> _sceneIsLoadSubject = new();

        public Observable<Unit> SceneIsLoad => _sceneIsLoadSubject;

        public void Construct(SceneLoader sceneLoader, SceneResources resources)
        {
            _resources = resources;
            _sceneLoader = sceneLoader;
        }

        public async UniTask LoadScene(SceneGroup sceneGroup, IProgress<float> progress, TypeScene typeScene)
        {
            var sceneRef = sceneGroup.FindSceneByReference(typeScene);
            
            if (sceneRef.State == SceneReferenceState.Addressable)
            {
                await LoadAddressableScene(sceneRef.Path, sceneGroup, progress);
            }
        }

        public async UniTask<AsyncOperationHandle<SceneInstance>> LoadLoadingScene(SceneGroup sceneGroup)
        {
            var scene = sceneGroup.FindSceneByReference(TypeScene.Loading);
            
            var sceneLoadOperation = Addressables.LoadSceneAsync(
                scene.Path,
                LoadSceneMode.Additive,
                false
            );
            
            await sceneLoadOperation.Task;
            
            if (sceneLoadOperation.Status == AsyncOperationStatus.Failed)
            {
                Debug.LogError("Failed to load scene: " + scene.Path);
                return sceneLoadOperation;
            }

            var sceneInstance = sceneLoadOperation.Result;

            if (!sceneInstance.Scene.IsValid())
            {
                Debug.LogWarning("Invalid scene instance loaded.");
            }

            await sceneInstance.ActivateAsync();

            SceneManager.SetActiveScene(sceneInstance.Scene); 
            return sceneLoadOperation;
        }

        public async UniTask UnloadScene()
        {
            if (_currentSceneHandle.IsValid())
            {
                await Addressables.UnloadSceneAsync(_currentSceneHandle);
                
                _currentSceneHandle = default;
                
                await Resources.UnloadUnusedAssets();
            }
        }

        public void UnloadResources()
        {
            foreach (var resource in _resources.ObjectToRelease)
                Addressables.ReleaseInstance(resource);
            
            _resources.ClearObject();
        }

        private async UniTask<bool> LoadAddressableScene(string scenePath, SceneGroup sceneGroup, IProgress<float> progress)
        {
            var initSceneName = sceneGroup.FindSceneByReference(TypeScene.InitialScene).Name;
            
            var initScene = SceneManager.GetSceneByName(initSceneName);
            var hadInit = initScene.IsValid() && initScene.isLoaded;

            var handle = Addressables.LoadSceneAsync(
                scenePath,
                LoadSceneMode.Additive,
                false
                );
            
            _currentSceneHandle = handle;

            var fakeProgress = 0f;
            
            while (_sceneLoader.Progress.Value < 1 || ! handle.IsDone)
            {
                var target = Mathf.Clamp01(handle.PercentComplete);
                fakeProgress = Mathf.MoveTowards(fakeProgress, target, 0.01f);
                progress.Report(fakeProgress);

                await UniTask.Yield();
            }

            if (handle.Status == AsyncOperationStatus.Failed)
            {
                Debug.LogError($"[SceneService] Failed to load scene: {scenePath}");
                return false;
            }
            
            var sceneInstance = handle.Result;
            if (!sceneInstance.Scene.IsValid())
            {
                Debug.LogWarning("Invalid scene instance loaded.");
                return false;
            }

            await sceneInstance.ActivateAsync();
            
            SceneManager.SetActiveScene(sceneInstance.Scene);
            
            _sceneLoader.IsLoading.Value = false;
            
            if (hadInit)
            {
                await SceneManager.UnloadSceneAsync(initScene);
            }

            _sceneLoader.IsLoading.Value = false;
            _sceneIsLoadSubject.OnNext(Unit.Default);
            
            return true;
        }
    }
}
