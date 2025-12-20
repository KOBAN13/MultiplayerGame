using System.Linq;
using Cysharp.Threading.Tasks;
using Db;
using GameCore.Factories;
using Services.SceneManagement;
using UI.Base;
using UnityEngine;
using VContainer;

namespace Factories
{
    public class ScreensFactory : Factory
    {
        [Inject] private ViewsFactory _viewsFactory;
        [Inject] private ScreensData _screensData;
        [Inject] private SceneResources _sceneResources;
        
        private Transform _parent;

        public void Initialize()
        {
            _parent = Object.Instantiate(_screensData.Root, null).transform;
        }

        public async UniTask<TView> CreateAsync<TView>() where TView : View
        {
            var data = _screensData.Screens.
                FirstOrDefault(d => d.Type == typeof(TView));
            var handle = await data.Asset.LoadAssetAsync<GameObject>();
            _sceneResources.AddObjectToRelease(handle);
            var prefab = handle.GetComponent<TView>();
            var screen = _viewsFactory.Create(prefab, _parent);
            screen.gameObject.SetActive(false);
            return screen;
        }
        
        public TView CreateSync<TView>() where TView : View
        {
            var data = _screensData.Screens.
                FirstOrDefault(d => d.Type == typeof(TView));
            var handle = data.Asset.LoadAssetAsync<GameObject>();
            var obj = handle.WaitForCompletion();
            var prefab = obj.GetComponent<TView>();
            var screen = _viewsFactory.Create(prefab, _parent);
            screen.gameObject.SetActive(false);
            return screen;
        }
    }
}