using System;
using Factories;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace UI.Base
{
    public abstract class View<TViewModel> : View
        where TViewModel : ViewModel, new()
    {
        [Inject] private ViewModelFactory _viewModelFactory;

        private ViewBinder[] _viewBinders;
        public TViewModel ViewModel { get; private set; }
        
        protected void Bind<TK>(params ViewBinder[] viewBinders) where TK : LifetimeScope
        {
            ViewModel = _viewModelFactory.Create<TViewModel, TK>(viewBinders);

            BindGuid();

            ViewModel.Initialize();
            
            foreach (var viewBinder in viewBinders)
            {
                if (ViewModel.ViewModelBinders.TryGetValue(viewBinder.AutoBindId.GeneratedGuid, out var viewModelBinder))
                {
                    viewBinder.ViewModelBinder = viewModelBinder;
                }
            }
            
            _viewBinders = viewBinders;
            
            foreach (var viewBinder in viewBinders)
            {
                viewBinder.Initialize();
            }
        }

        public abstract void BindGuid();

        public override void ApplyPayload<TPayload>(TPayload payload)
        {
            base.ApplyPayload(payload);

            if (ViewModel is IPayloadReceiver<TPayload> viewModelReceiver)
            {
                viewModelReceiver.ApplyPayload(payload);
            }
        }

        public override void Dispose()
        {
            foreach (var viewBinder in _viewBinders)
            {
                viewBinder.Dispose();
            }
        }
    }

    public abstract class View : MonoBehaviour, IDisposable
    {
        public abstract void Initialize();

        public virtual void Open()
        {
            gameObject.SetActive(true);
        }

        public virtual void Close()
        {
            gameObject.SetActive(false);
        }

        public virtual void ApplyPayload<TPayload>(TPayload payload)
        {
            if (this is IPayloadReceiver<TPayload> payloadReceiver)
            {
                payloadReceiver.ApplyPayload(payload);
            }
        }

        public virtual void Dispose()
        {
        }
    }
}