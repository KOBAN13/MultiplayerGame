using System;
using R3;
using UI.Base;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Binders
{
    [Serializable]
    public class ToggleViewBinder : ViewBinder<ReactiveCommand<bool>>
    {
        [SerializeField] private Toggle _toggle;
        
        private ReactiveCommand<bool> _reactiveCommand;
        
        public override void Parse(ReactiveCommand<bool> value)
        {
            _reactiveCommand = value;
            _toggle.onValueChanged.AddListener(OnChange);
        }
        
        public override void Dispose()
        {
            base.Dispose();
            
            _toggle.onValueChanged.RemoveListener(OnChange);
            _reactiveCommand = null;
        }

        private void OnChange(bool value)
        {
            _reactiveCommand.Execute(value);
        }
    }
}