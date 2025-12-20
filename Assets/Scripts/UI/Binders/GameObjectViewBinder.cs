using System;
using UI.Base;
using UI.Utils;
using UnityEngine;

namespace UI.Binders
{
    [Serializable]
    public class GameObjectViewBinder : ViewBinder<EUIObjectState>
    {
        [SerializeField] private GameObject _gameObject;
        
        public override void Parse(EUIObjectState value)
        {
            switch (value)
            {
                case EUIObjectState.Show:
                    _gameObject.SetActive(true);
                    break;
                case EUIObjectState.Hide:
                    _gameObject.SetActive(false);
                    break;
                case EUIObjectState.None:
                    break;
            }
            
        }
    }
}