using System;
using TMPro;
using UI.Base;
using UnityEngine;

namespace UI.Binders
{
    [Serializable]
    public class TextViewBinder : ViewBinder<string>
    {
        [SerializeField] private TMP_Text _testText;
        
        public override void Parse(string value)
        {
            _testText.text = value;
        }
    }
}