using System;
using UI.Helpers;
using UnityEngine;

namespace UI.Base
{
    public abstract class ViewBinder<TViewModelValue> : ViewBinder
    {
        private ViewModelBinder<TViewModelValue> _viewModelBinder;

        public override void Initialize()
        {
            if (ViewModelBinder is ViewModelBinder<TViewModelValue> viewModelBinder)
            {
                _viewModelBinder = viewModelBinder;
                _viewModelBinder.SubscribeParse(Parse);
            }
        }

        public abstract void Parse(TViewModelValue value);
        
        public override void Dispose()
        {
            _viewModelBinder.DisposeParse();
        }
    }
    
    [Serializable]
    public abstract class ViewBinder : IDisposable
    {
        [field: SerializeField]
        public AutoBindId AutoBindId { get; set; } = new();
        
        public ViewModelBinder ViewModelBinder;

        public abstract void Initialize();
        
        public virtual void Dispose()
        {
            
        }
    }
}
