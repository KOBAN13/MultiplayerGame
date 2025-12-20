using Di;
using Helpers;
using UI.Base;
using UI.Binders;
using UI.Helpers;
using UI.ViewModel;
using UnityEngine;

namespace UI.View
{
    public class MainMenuScreen : Screen<MainMenuViewModel>
    {
        [SerializeField, AutoBind] private ButtonViewBinder _signUpButton = new();
        [SerializeField, AutoBind] private ButtonViewBinder _signInButton = new();
        [SerializeField, AutoBind] private ButtonViewBinder _playButton = new();
        [SerializeField, AutoBind] private TextViewBinder _usernameBinder = new();
        [SerializeField, AutoBind] private GameObjectViewBinder _objectLoginPanel = new();
        [SerializeField, AutoBind] private GameObjectViewBinder _objectUsernamePanel = new();
        [SerializeField, AutoBind] private ButtonViewBinder _autoLoginButton = new();
        
        public override void Initialize()
        {
            Bind<MainMenuTimeScope>(_signUpButton, 
                _signInButton, 
                _playButton,
                _objectLoginPanel,
                _objectUsernamePanel,
                _usernameBinder,
                _autoLoginButton
            );
        }

        public override void BindGuid()
        {
            ViewModel.SignUpButtonViewBinder.AutoBindId.SetGuid(_signUpButton.AutoBindId.GeneratedGuid);
            ViewModel.SignInButtonViewBinder.AutoBindId.SetGuid(_signInButton.AutoBindId.GeneratedGuid);
            ViewModel.PlayButtonViewBinder.AutoBindId.SetGuid(_playButton.AutoBindId.GeneratedGuid);
            ViewModel.ObjectLoginPanel.AutoBindId.SetGuid(_objectLoginPanel.AutoBindId.GeneratedGuid);
            ViewModel.ObjectUsernamePanel.AutoBindId.SetGuid(_objectUsernamePanel.AutoBindId.GeneratedGuid);
            ViewModel.UserNameBinder.AutoBindId.SetGuid(_usernameBinder.AutoBindId.GeneratedGuid);
            ViewModel.AutoLoginButtonViewBinder.AutoBindId.SetGuid(_autoLoginButton.AutoBindId.GeneratedGuid);
        }
    }
}