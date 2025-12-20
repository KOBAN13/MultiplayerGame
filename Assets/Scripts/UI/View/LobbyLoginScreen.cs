using Di;
using R3;
using UI.Base;
using UI.Binders;
using UI.Helpers;
using UI.ViewModel;
using UnityEngine;

namespace UI.View
{
    public class LobbyLoginScreen : Screen<LobbyLoginViewModel>
    {
        [SerializeField, AutoBind] private InputFieldTextChangedViewBinder _passwordBinder = new();
        [SerializeField, AutoBind] private ButtonViewBinder _buttonViewBinder = new();
        [SerializeField, AutoBind] private ButtonViewBinder _closeScreenButtonBinder = new();
        [SerializeField, AutoBind] private GameObjectViewBinder _errorTextObject = new();
        [SerializeField, AutoBind] private TextViewBinder _errorTextBinder = new();
        
        public override void Initialize()
        {
            Bind<LobbyTimeScope>(
                _passwordBinder, 
                _buttonViewBinder,
                _errorTextObject,
                _errorTextBinder,
                _closeScreenButtonBinder);
            
            ViewModel.InteractableSignInButton.Subscribe(OnInteractableSignInButton).AddTo(this);
        }

        private void OnInteractableSignInButton(bool value)
        {
            Debug.Log($"InteractableSignInButton: {value}");
            _buttonViewBinder.Button.interactable = value;
        }

        public override void BindGuid()
        {
            ViewModel.PasswordBinder.AutoBindId.SetGuid(_passwordBinder.AutoBindId.GeneratedGuid);
            ViewModel.ButtonViewBinder.AutoBindId.SetGuid(_buttonViewBinder.AutoBindId.GeneratedGuid);
            ViewModel.ErrorTextObjectBinder.AutoBindId.SetGuid(_errorTextObject.AutoBindId.GeneratedGuid);
            ViewModel.ErrorTextBinder.AutoBindId.SetGuid(_errorTextBinder.AutoBindId.GeneratedGuid);
            ViewModel.CloseScreenButtonBinder.AutoBindId.SetGuid(_closeScreenButtonBinder.AutoBindId.GeneratedGuid);
        }
    }
}