using Di;
using UI.Base;
using UI.Binders;
using UI.ViewModel;
using R3;
using UI.Helpers;
using UnityEngine;

namespace UI.View
{
    public class RestorePasswordScreen : Screen<RestorePasswordViewModel>
    {
        [SerializeField, AutoBind] private InputFieldTextChangedViewBinder _emailBinder = new();
        [SerializeField, AutoBind] private TextViewBinder _errorBinder = new();
        [SerializeField, AutoBind] private GameObjectViewBinder _objectLoginError = new();
        [SerializeField, AutoBind] private ButtonViewBinder _restoreInBinder = new();
        [SerializeField, AutoBind] private ButtonViewBinder _closeBinder = new();
        
        public override void Initialize()
        {
            Bind<MainMenuTimeScope>(_emailBinder,
                _errorBinder,
                _objectLoginError,
                _restoreInBinder,
                _closeBinder
            );
            
            ViewModel.InteractableSignInButton.Subscribe(OnInteractableSignInButton).AddTo(this);
        }
        
        private void OnInteractableSignInButton(bool value) => _restoreInBinder.Button.interactable = value;
        
        public override void BindGuid()
        {
            ViewModel.CloseBinder.AutoBindId.SetGuid(_closeBinder.AutoBindId.GeneratedGuid);
            ViewModel.RestoreBinder.AutoBindId.SetGuid(_restoreInBinder.AutoBindId.GeneratedGuid);
            ViewModel.ObjectLoginPanel.AutoBindId.SetGuid(_objectLoginError.AutoBindId.GeneratedGuid);
            ViewModel.ErrorBinder.AutoBindId.SetGuid(_errorBinder.AutoBindId.GeneratedGuid);
            ViewModel.EmailBinder.AutoBindId.SetGuid(_emailBinder.AutoBindId.GeneratedGuid);
        }
    }
}