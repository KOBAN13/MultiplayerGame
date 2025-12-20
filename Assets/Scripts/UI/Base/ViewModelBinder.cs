using System;
using R3;
using UI.Helpers;
using UnityEngine;

namespace UI.Base
{
    public class ViewModelBinder<TValue> : ViewModelBinder
    {
        protected TValue _value;
        protected ReactiveProperty<TValue> _binderTriggered = new();
        private readonly CompositeDisposable _disposable = new();

        public TValue Value
        {
            get => _value;
            set
            {
                _value = value;
                _binderTriggered.Value = value;
            }
        }
        
        public void SubscribeParse(Action<TValue> action)
        {
            _binderTriggered.Subscribe(value => action?.Invoke(value)).AddTo(_disposable);
        }

        public void DisposeParse()
        {
            _disposable.Dispose();
        }
    }
    
    public class RefTypeViewModelBinder<TValue> : ViewModelBinder<TValue> where TValue : class, new()
    {
        public RefTypeViewModelBinder()
        {
            _value = new ();
            _binderTriggered.Value = _value;
        }
    }
    
    [Serializable]
    public class ViewModelBinder
    {
        [field: SerializeField]
        public AutoBindId AutoBindId { get; set; } = new();
    }
}