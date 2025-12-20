using System;
using Cysharp.Threading.Tasks;
using R3;
using Services.SceneManagement;
using Services.SceneManagement.Enums;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Services.Interface
{
    public interface IScenesService
    {
        UniTask<AsyncOperationHandle<SceneInstance>> LoadLoadingScene(SceneGroup sceneGroup);
        void UnloadResources();
        UniTask UnloadScene();
        UniTask LoadScene(SceneGroup sceneGroup, IProgress<float> progress, TypeScene typeScene);
        void Construct(SceneLoader loader, SceneResources resources);
        Observable<Unit> SceneIsLoad { get; }
    }
}