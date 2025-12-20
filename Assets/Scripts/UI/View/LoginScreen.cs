using Di;
using Helpers;
using UI.Base;
using UI.Binders;
using UI.ViewModel;
using R3;
using UI.Helpers;
using UnityEngine;

namespace UI.View
{
    public class LoginScreen : Screen<LoginViewModel>
    {
        [SerializeField, AutoBind] private InputFieldTextChangedViewBinder _loginBinder = new();
        [SerializeField, AutoBind] private InputFieldTextChangedViewBinder _passwordBinder = new();
        [SerializeField, AutoBind] private TextViewBinder _errorBinder = new();
        [SerializeField, AutoBind] private GameObjectViewBinder _objectLoginError = new();
        
        [SerializeField, AutoBind] private ButtonViewBinder _signInBinder = new();
        [SerializeField, AutoBind] private ButtonViewBinder _closeBinder = new();
        [SerializeField, AutoBind] private ButtonViewBinder _restorePasswordBinder = new();

        public override void Initialize()
        {
            Bind<MainMenuTimeScope>(_loginBinder,
                _passwordBinder, 
                _signInBinder,
                _closeBinder,
                _errorBinder, 
                _objectLoginError,
                _restorePasswordBinder
            );
            
            ViewModel.InteractableSignInButton.Subscribe(OnInteractableSignInButton).AddTo(this);
        }
        
        private void OnInteractableSignInButton(bool value) => _signInBinder.Button.interactable = value;

        public override void BindGuid()
        {
            ViewModel._loginBinder.AutoBindId.SetGuid(_loginBinder.AutoBindId.GeneratedGuid);
            ViewModel._passwordBinder.AutoBindId.SetGuid(_passwordBinder.AutoBindId.GeneratedGuid);
            ViewModel._errorBinder.AutoBindId.SetGuid(_errorBinder.AutoBindId.GeneratedGuid);
            ViewModel._objectLoginPanel.AutoBindId.SetGuid(_objectLoginError.AutoBindId.GeneratedGuid);
            ViewModel._signInBinder.AutoBindId.SetGuid(_signInBinder.AutoBindId.GeneratedGuid);
            ViewModel._closeBinder.AutoBindId.SetGuid(_closeBinder.AutoBindId.GeneratedGuid);
            ViewModel._restorePasswordBinder.AutoBindId.SetGuid(_restorePasswordBinder.AutoBindId.GeneratedGuid);
        }
    }
}