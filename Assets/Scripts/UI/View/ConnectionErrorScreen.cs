using Di;
using Helpers;
using UI.Base;
using UI.Binders;
using UI.Helpers;
using UI.ViewModel;
using UnityEngine;

namespace UI.View
{
    public class ConnectionErrorScreen : Screen<ConnectionErrorViewModel>
    {
        [SerializeField, AutoBind] private TextViewBinder _textViewBinder = new();
        [SerializeField, AutoBind] private ButtonViewBinder _buttonViewBinder = new();
        
        public override void Initialize()
        {
            Bind<MainMenuTimeScope>(_textViewBinder, _buttonViewBinder);
        }

        public override void BindGuid()
        {
            ViewModel.TextViewBinder.AutoBindId.SetGuid(_textViewBinder.AutoBindId.GeneratedGuid);
            ViewModel.ButtonViewBinder.AutoBindId.SetGuid(_buttonViewBinder.AutoBindId.GeneratedGuid);
        }
    }
}