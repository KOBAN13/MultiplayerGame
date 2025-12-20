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
    public class SignUpViewModel : Base.ViewModel
    {
        [Inject] 
        private IRegistrationService _registrationService;
        
        [Inject] 
        private IScreenService _screenService;
        
        [AutoBind]
        public readonly RefTypeViewModelBinder<ReactiveCommand<string>> EmailBinder = new();
        
        [AutoBind]
        public readonly RefTypeViewModelBinder<ReactiveCommand<string>> LoginBinder = new();
        
        [AutoBind]
        public readonly RefTypeViewModelBinder<ReactiveCommand<string>> PasswordBinder = new();
        
        [AutoBind]
        public readonly RefTypeViewModelBinder<ReactiveCommand> SignInBinder = new();
        
        [AutoBind]
        public readonly RefTypeViewModelBinder<ReactiveCommand> CloseBinder = new();
        
        [AutoBind]
        public readonly RefTypeViewModelBinder<ReactiveCommand> RestorePasswordBinder = new();
        
        [AutoBind]
        public readonly ViewModelBinder<string> ErrorBinder = new();
        
        [AutoBind]
        public readonly ViewModelBinder<EUIObjectState> ObjectLoginPanel = new();
        
        private readonly ReactiveProperty<bool> _interactableSignInButton = new(true);
        
        public ReadOnlyReactiveProperty<bool> InteractableSignInButton => _interactableSignInButton;
        
        private string _email = string.Empty;
        private string _login = string.Empty;
        private string _password = string.Empty;
        
        public override void Initialize()
        {
            Bind(EmailBinder,
                LoginBinder,
                PasswordBinder,
                SignInBinder,
                CloseBinder,
                ErrorBinder,
                ObjectLoginPanel,
                RestorePasswordBinder
            );
            
            EmailBinder.Value.Subscribe(OnEmailChanged).AddTo(Disposable);
            LoginBinder.Value.Subscribe(OnLoginChanged).AddTo(Disposable);
            PasswordBinder.Value.Subscribe(OnPasswordChanged).AddTo(Disposable);
            SignInBinder.Value.Subscribe(OnLoginRequest).AddTo(Disposable);
            CloseBinder.Value.Subscribe(OnCloseScreen).AddTo(Disposable);
            RestorePasswordBinder.Value.Subscribe(OnRestorePasswordScreen).AddTo(Disposable);
            _registrationService.RegisterError.Subscribe(OnError).AddTo(Disposable);
            _registrationService.SuccessRegistration.Subscribe(OnRegistration).AddTo(Disposable);
        }

        private void OnError(string error)
        {
            if (string.IsNullOrEmpty(error))
                return;
            
            ErrorBinder.Value = error;
            ObjectLoginPanel.Value = EUIObjectState.Show;
            _interactableSignInButton.Value = true;
        }

        private void OnRegistration(Unit unit)
        {
            ObjectLoginPanel.Value = EUIObjectState.Hide;
            _interactableSignInButton.Value = true;
            _screenService.CloseScreen<SignUpScreen>();
        }
        
        private async void OnRestorePasswordScreen(Unit unit) => await _screenService.OpenAsync<RestorePasswordScreen>();
        private void OnCloseScreen(Unit unit) => _screenService.CloseScreen<SignUpScreen>();
        private void OnEmailChanged(string email) => _email = email;
        private void OnLoginChanged(string login) => _login = login;
        private void OnPasswordChanged(string password) => _password = password;

        private void OnLoginRequest(Unit unit)
        {
            _interactableSignInButton.Value = false;
            
            _registrationService.Register(_email, _login, _password);
        }
    }
}