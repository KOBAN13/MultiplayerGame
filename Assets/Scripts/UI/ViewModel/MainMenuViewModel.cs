using Helpers;
using R3;
using Services.Interface;
using Services.SceneManagement;
using Services.SceneManagement.Enums;
using UI.Base;
using UI.Helpers;
using UI.Utils;
using UI.View;
using VContainer;

namespace UI.ViewModel
{
    public class MainMenuViewModel : Base.ViewModel
    {
        [Inject] 
        private ILoginClientService _loginClientService;
        
        [Inject] 
        private IScreenService _screenService;
        
        [Inject] 
        private INoteService _noteService;
        
        [Inject] 
        private SceneLoader _sceneLoader;
        
        [AutoBind]
        public readonly RefTypeViewModelBinder<ReactiveCommand> SignInButtonViewBinder = new ();
        
        [AutoBind]
        public readonly RefTypeViewModelBinder<ReactiveCommand> SignUpButtonViewBinder = new ();
        
        [AutoBind]
        public readonly RefTypeViewModelBinder<ReactiveCommand> PlayButtonViewBinder = new ();
        
        [AutoBind]
        public readonly RefTypeViewModelBinder<ReactiveCommand> AutoLoginButtonViewBinder = new ();
        
        [AutoBind]
        public readonly ViewModelBinder<string> UserNameBinder = new ();
        
        [AutoBind]
        public readonly ViewModelBinder<EUIObjectState> ObjectLoginPanel = new ();
        
        [AutoBind]
        public readonly ViewModelBinder<EUIObjectState> ObjectUsernamePanel = new ();
        
        public override void Initialize()
        {
            Bind(SignInButtonViewBinder,
                SignUpButtonViewBinder, 
                PlayButtonViewBinder, 
                ObjectLoginPanel, 
                ObjectUsernamePanel,
                UserNameBinder,
                AutoLoginButtonViewBinder
            );
            
            SignUpButtonViewBinder.Value.Subscribe(SignUp).AddTo(Disposable);
            SignInButtonViewBinder.Value.Subscribe(SignIn).AddTo(Disposable);
            PlayButtonViewBinder.Value.Subscribe(Play).AddTo(Disposable);
            AutoLoginButtonViewBinder.Value.Subscribe(AutoLogin).AddTo(Disposable);
            _loginClientService.UserName.Subscribe(OnUserNameChanged).AddTo(Disposable);
        }
        
        private async void SignUp(Unit unit) => await _screenService.OpenAsync<SignUpScreen>();
        private async void SignIn(Unit unit) => await _screenService.OpenAsync<LoginScreen>();

        private async void Play(Unit unit)
        {
            if (_loginClientService.IsUserLogin())
            {
                await _sceneLoader.LoadScene(TypeScene.Lobby);
            }
            else
            {
                _noteService.UpdateNote("Connect", "Login to play");
                
                await _screenService.OpenAsync<NoteScreen>();
            }
        }
        
        private void AutoLogin(Unit unit)
        {
            var login = "Daniil";
            var password = "giftrola05";
            
            _loginClientService.Login(login, password);
        }

        private void OnUserNameChanged(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                return;
            
            UserNameBinder.Value = userName;
            ObjectLoginPanel.Value = EUIObjectState.Hide;
            ObjectUsernamePanel.Value = EUIObjectState.Show;
        }
    }
}