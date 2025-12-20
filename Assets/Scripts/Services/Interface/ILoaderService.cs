using Cysharp.Threading.Tasks;
using Services.Helpers;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Services.Interface
{
    public interface ILoaderService
    {
        UniTask<UploadedResources<T>> LoadResources<T>(string nameResources);
        UniTask<UploadedResources<T>> LoadResourcesUsingReference<T>(AssetReferenceT<T> resource) where T : Object;
        UniTask<UploadedResourcesList<T>> LoadAllResourcesUseLabel<T>(AssetLabelReference labelReference) where T : Object;
        void ClearMemory<T>(AsyncOperationHandle<T> handle);
        void ClearMemoryInstance(GameObject objectClear);
    }
}