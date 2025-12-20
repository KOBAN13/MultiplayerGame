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
    public class SignUpScreen : Screen<SignUpViewModel>
    {
        [SerializeField, AutoBind] private InputFieldTextChangedViewBinder _emailBinder = new();
        [SerializeField, AutoBind] private InputFieldTextChangedViewBinder _loginBinder = new();
        [SerializeField, AutoBind] private InputFieldTextChangedViewBinder _passwordBinder = new();
        [SerializeField, AutoBind] private TextViewBinder _errorBinder = new();
        [SerializeField, AutoBind] private GameObjectViewBinder _objectLoginError = new();
        [SerializeField, AutoBind] private ButtonViewBinder _signInBinder = new();
        [SerializeField, AutoBind] private ButtonViewBinder _closeBinder = new();
        [SerializeField, AutoBind] private ButtonViewBinder _restorePasswordBinder = new();
        
        public override void Initialize()
        {
            Bind<MainMenuTimeScope>(_emailBinder,
                _loginBinder,
                _passwordBinder,
                _errorBinder,
                _objectLoginError,
                _signInBinder,
                _closeBinder,
                _restorePasswordBinder
            );
            
            ViewModel.InteractableSignInButton.Subscribe(OnInteractableSignInButton).AddTo(this);
        }
        
        private void OnInteractableSignInButton(bool value) => _signInBinder.Button.interactable = value;
        
        public override void BindGuid()
        {
            ViewModel.EmailBinder.AutoBindId.SetGuid(_emailBinder.AutoBindId.GeneratedGuid);
            ViewModel.LoginBinder.AutoBindId.SetGuid(_loginBinder.AutoBindId.GeneratedGuid);
            ViewModel.PasswordBinder.AutoBindId.SetGuid(_passwordBinder.AutoBindId.GeneratedGuid);
            ViewModel.SignInBinder.AutoBindId.SetGuid(_signInBinder.AutoBindId.GeneratedGuid);
            ViewModel.CloseBinder.AutoBindId.SetGuid(_closeBinder.AutoBindId.GeneratedGuid);
            ViewModel.RestorePasswordBinder.AutoBindId.SetGuid(_restorePasswordBinder.AutoBindId.GeneratedGuid);
            ViewModel.ErrorBinder.AutoBindId.SetGuid(_errorBinder.AutoBindId.GeneratedGuid);
        }
    }
}