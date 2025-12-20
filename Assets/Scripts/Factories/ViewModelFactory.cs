using System;
using GameCore.Factories;
using UI.Base;
using VContainer;
using VContainer.Unity;

namespace Factories
{
    public class ViewModelFactory : Factory
    {
        private IObjectResolver _resolver;
        
        public T Create<T, TK>(params ViewBinder[] viewBinders) where T : ViewModel, new() where TK: LifetimeScope
        {
            _resolver = LifetimeScope.Find<TK>().Container;
            
            var viewModel = new T();
            
            _resolver.Inject(viewModel);
            
            return viewModel;
        }
    }
}
