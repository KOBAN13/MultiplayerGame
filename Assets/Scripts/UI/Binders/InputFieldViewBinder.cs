using System;
using R3;
using TMPro;
using UI.Base;
using UnityEngine;

namespace UI.Binders
{
    [Serializable]
    public class InputFieldTextChangedViewBinder : InputFieldViewBinder<ReactiveCommand<string>>
    {
        private ReactiveCommand<string> _reactiveCommand;
        
        public override void Parse(ReactiveCommand<string> value)
        {
            _reactiveCommand = value;
            InputField.onValueChanged.AddListener(OnChanged);
        }

        public override void Dispose()
        {
            base.Dispose();
            
            InputField.onValueChanged.RemoveListener(OnChanged);
            _reactiveCommand = null;
        }

        private void OnChanged(string value)
        {
            _reactiveCommand.Execute(value);
        }
    }
    
    [Serializable]
    public class InputFieldSetTextViewBinder : InputFieldViewBinder<string>
    {
        public override void Parse(string value)
        {
            InputField.text = value;
        }
    }
    
    [Serializable]
    public class InputFieldViewBinder<TValue> : ViewBinder<TValue>
    {
        [SerializeField] protected TMP_InputField InputField;

        public override void Parse(TValue value)
        {
            
        }
    }
}