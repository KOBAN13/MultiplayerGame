using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Services.Interface;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;


namespace Services.Helpers
{
    public class LoaderService : ILoaderService
    {
        public async UniTask<UploadedResources<T>> LoadResources<T>(string nameResources)
        {
            UniTaskCompletionSource<T> isTaskCompletion = new();
            AsyncOperationHandle<T> operationHandle = default;

            try
            {
                operationHandle = Addressables.LoadAssetAsync<T>(nameResources);
                await operationHandle.Task.AsUniTask();

                if (operationHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    isTaskCompletion.TrySetResult(operationHandle.Result);
                }
                else isTaskCompletion.TrySetException(new Exception("Failed load asset"));
            }
            catch (Exception exception)
            {
                isTaskCompletion.TrySetException(exception);
            }

            return new UploadedResources<T>(operationHandle, await isTaskCompletion.Task);
        }

        public async UniTask<UploadedResources<T>> LoadResourcesUsingReference<T>(AssetReferenceT<T> resource)
            where T : Object
        {
            UniTaskCompletionSource<T> isTaskCompletion = new();
            AsyncOperationHandle<T> operationHandle = default;

            try
            {
                operationHandle = resource.LoadAssetAsync();
                await operationHandle;

                if (operationHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    isTaskCompletion.TrySetResult(operationHandle.Result);
                }

                isTaskCompletion.TrySetException(new Exception("Failed load asset"));
            }
            catch (Exception e)
            {
                isTaskCompletion.TrySetException(e);
            }

            return new UploadedResources<T>(operationHandle, await isTaskCompletion.Task);
        }

        public async UniTask<UploadedResourcesList<T>> LoadAllResourcesUseLabel<T>(AssetLabelReference labelReference)
            where T : Object
        {
            UniTaskCompletionSource<List<T>> isTaskCompletionSource = new();
            AsyncOperationHandle<IList<T>> operationHandle = default;

            try
            {
                operationHandle = Addressables.LoadAssetsAsync<T>(labelReference,
                    (objectLoad) => { Debug.Log($"{objectLoad.GetType()} is load"); });

                await operationHandle;

                if (operationHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    isTaskCompletionSource.TrySetResult((List<T>)operationHandle.Result);
                }

                isTaskCompletionSource.TrySetException(new Exception("Failed load asset"));
            }
            catch (Exception e)
            {
                isTaskCompletionSource.TrySetException(e);
            }

            return new UploadedResourcesList<T>(operationHandle, await isTaskCompletionSource.Task);
        }

        public void ClearMemory<T>(AsyncOperationHandle<T> handle)
        {
            Addressables.Release(handle);
        }

        public void ClearMemoryInstance(GameObject objectClear)
        {
            Addressables.ReleaseInstance(objectClear);
        }
    }

    public class UploadedResources<T>
    {
        public T resources;
        public AsyncOperationHandle<T> operationHandle;

        public UploadedResources(AsyncOperationHandle<T> operationHandle, T resources)
        {
            this.operationHandle = operationHandle;
            this.resources = resources;
        }
    }

    public class UploadedResourcesList<T>
    {
        public List<T> resources;
        public AsyncOperationHandle<IList<T>> operationHandle;

        public UploadedResourcesList(AsyncOperationHandle<IList<T>> operationHandle, List<T> resources)
        {
            this.operationHandle = operationHandle;
            this.resources = resources;
        }
    }
}