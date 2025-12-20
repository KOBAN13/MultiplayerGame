using GameCore.Factories;
using UI.Base;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Factories
{
    public class ViewsFactory : Factory
    {
        private readonly IObjectResolver _resolver;
        
        public ViewsFactory(LifetimeScope scope)
        {
            _resolver = scope.Container;
        }
        
        public TView Create<TView>(TView prefab, Transform parent)
            where TView : View
        {
            var view = Object.Instantiate(prefab, parent);
            InitializeView(view);
            return view;
        }

        public void InitializeView(View view)
        {
            _resolver.Inject(view);
            view.Initialize();
        }
    }
}