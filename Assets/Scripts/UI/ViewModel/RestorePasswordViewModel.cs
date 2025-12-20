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
    public class RestorePasswordViewModel : Base.ViewModel
    {
        [Inject] 
        private IScreenService _screenService;
        
        [Inject] 
        private IRestorePasswordService _restorePasswordService;
        
        [AutoBind]
        public readonly RefTypeViewModelBinder<ReactiveCommand<string>> EmailBinder = new();
        
        [AutoBind]
        public readonly RefTypeViewModelBinder<ReactiveCommand> RestoreBinder = new();
        
        [AutoBind]
        public readonly RefTypeViewModelBinder<ReactiveCommand> CloseBinder = new();
        
        [AutoBind]
        public readonly ViewModelBinder<string> ErrorBinder = new();
        
        [AutoBind]
        public readonly ViewModelBinder<EUIObjectState> ObjectLoginPanel = new();
        
        private readonly ReactiveProperty<bool> _interactableSignInButton = new(true);
        
        public ReadOnlyReactiveProperty<bool> InteractableSignInButton => _interactableSignInButton;
        
        private string _email;
        
        public override void Initialize()
        {
            Bind(EmailBinder,
                RestoreBinder, 
                CloseBinder, 
                ErrorBinder, 
                ObjectLoginPanel
            );
            
            EmailBinder.Value.Subscribe(OnEmailChanged).AddTo(Disposable);
            
            RestoreBinder.Value.Subscribe(OnRestoreRequest).AddTo(Disposable);
            
            CloseBinder.Value.Subscribe(OnCloseScreen).AddTo(Disposable);
            
            _restorePasswordService.RestoreResult.Subscribe(OnRestoreResult).AddTo(Disposable);
        }
        
        private void OnCloseScreen(Unit unit)
        {
            _screenService.CloseScreen<RestorePasswordScreen>();
        }

        private void OnEmailChanged(string email)
        {
            if (string.IsNullOrEmpty(email))
                return;
            
            _email = email;
        }

        private void OnRestoreRequest(Unit unit)
        {
            if (string.IsNullOrEmpty(_email))
                return;
            
            _interactableSignInButton.Value = false;
            
            _restorePasswordService.RestorePassword(_email);
        }
        
        private void OnRestoreResult(string error)
        {
            if (string.IsNullOrEmpty(error))
                return;
            
            ObjectLoginPanel.Value = EUIObjectState.Show;
            _interactableSignInButton.Value = true;
            ErrorBinder.Value = error;
        }
    }
}