using Cysharp.Threading.Tasks;
using Helpers;
using R3;
using Services.Interface;
using UI.Base;
using UI.Helpers;
using UI.Utils;
using UI.View;
using VContainer;

namespace UI.ViewModel
{
    public class LoginViewModel : Base.ViewModel
    {
        [Inject] 
        private ILoginClientService _loginClientService;
        
        [Inject]
        private IScreenService _screenService;
        
        [AutoBind]
        public readonly RefTypeViewModelBinder<ReactiveCommand<string>> _loginBinder = new();
        
        [AutoBind]
        public readonly RefTypeViewModelBinder<ReactiveCommand<string>> _passwordBinder = new();
        
        [AutoBind]
        public readonly RefTypeViewModelBinder<ReactiveCommand> _signInBinder = new();
        
        [AutoBind]
        public readonly RefTypeViewModelBinder<ReactiveCommand> _closeBinder = new();
        
        [AutoBind]
        public readonly RefTypeViewModelBinder<ReactiveCommand> _restorePasswordBinder = new();
        
        [AutoBind]
        public readonly ViewModelBinder<string> _errorBinder = new();
        
        [AutoBind]
        public readonly ViewModelBinder<EUIObjectState> _objectLoginPanel = new();

        private readonly ReactiveProperty<bool> _interactableSignInButton = new(true);
        
        public Observable<bool> InteractableSignInButton => _interactableSignInButton;

        private string _login;
        private string _password;
        
        public override void Initialize()
        {
            Bind(_loginBinder, _passwordBinder, _signInBinder, _closeBinder, _errorBinder, _objectLoginPanel, _restorePasswordBinder);
            
            _signInBinder.Value.Subscribe(OnLoginRequest).AddTo(Disposable);
            
            _loginBinder.Value.Subscribe(OnLoginChanged).AddTo(Disposable);
            
            _passwordBinder.Value.Subscribe(OnPasswordChanged).AddTo(Disposable);
            
            _closeBinder.Value.Subscribe(OnCloseScreen).AddTo(Disposable);
            
            
            _restorePasswordBinder.Value.Subscribe(OnRestorePasswordScreen).AddTo(Disposable);
            
            _loginClientService.LoginErrorRequest.Subscribe(OnLoginToLobbyError).AddTo(Disposable);
            
            _loginClientService.SuccessLogin.Subscribe(OnSuccessLoginToRoom).AddTo(Disposable);
        }
        
        private void OnLoginChanged(string login) => _login = login;
        
        private void OnPasswordChanged(string password) => _password = password;

        private void OnCloseScreen(Unit unit)
        {
            _screenService.CloseScreen<LoginScreen>();
        }

        private async void OnRestorePasswordScreen(Unit unit) => await _screenService.OpenAsync<RestorePasswordScreen>();

        private void OnLoginToLobbyError(string error)
        {
            if (string.IsNullOrEmpty(error))
                return;
            
            _errorBinder.Value = error;
            _objectLoginPanel.Value = EUIObjectState.Show;
            _interactableSignInButton.Value = true;
        }

        private void OnSuccessLoginToRoom(Unit unit)
        {
            _objectLoginPanel.Value = EUIObjectState.Hide;
            _interactableSignInButton.Value = true;
            _screenService.CloseScreen<LoginScreen>();
        }

        private void OnLoginRequest(Unit unit)
        { 
            if (string.IsNullOrEmpty(_login) || string.IsNullOrEmpty(_password)) 
                return;

            _interactableSignInButton.Value = false;
            
            _loginClientService.Login(_login, _password);
        }
    }
}