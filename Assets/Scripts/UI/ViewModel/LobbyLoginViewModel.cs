using Db;
using R3;
using Services.Interface;
using UI.Base;
using UI.Helpers;
using UI.Utils;
using UI.View;
using UnityEngine;
using VContainer;

namespace UI.ViewModel
{
    public class LobbyLoginViewModel : Base.ViewModel, IPayloadReceiver<RoomData>
    {
        [Inject] private ILobbyService _lobbyService;
        [Inject] private IScreenService _screenService;
        
        [AutoBind]
        public readonly RefTypeViewModelBinder<ReactiveCommand<string>> PasswordBinder = new();
        
        [AutoBind]
        public readonly RefTypeViewModelBinder<ReactiveCommand> ButtonViewBinder = new();
        
        [AutoBind]
        public readonly ViewModelBinder<string> ErrorTextBinder = new();
        
        [AutoBind]
        public readonly ViewModelBinder<EUIObjectState> ErrorTextObjectBinder = new();
        
        [AutoBind]
        public readonly RefTypeViewModelBinder<ReactiveCommand> CloseScreenButtonBinder = new();
        
        private readonly ReactiveProperty<bool> _interactableSignInButton = new(true);
        
        public Observable<bool> InteractableSignInButton => _interactableSignInButton;
        
        private string _password;
        private RoomData _roomData;

        public override void Initialize()
        {
            Bind(PasswordBinder, ButtonViewBinder, ErrorTextBinder, ErrorTextObjectBinder, CloseScreenButtonBinder);
            
            PasswordBinder.Value.Subscribe(OnPasswordChanged).AddTo(Disposable);
            ButtonViewBinder.Value.Subscribe(OnButtonLogin).AddTo(Disposable);
            CloseScreenButtonBinder.Value.Subscribe(OnCloseScreenButton).AddTo(Disposable);
            
            _lobbyService.JoinLobbySuccess.Subscribe(OnSuccessLogin).AddTo(Disposable);
            _lobbyService.JoinLobbyError.Subscribe(OnLoginError).AddTo(Disposable);
        }
        
        public void ApplyPayload(RoomData payload)
        {
            _roomData = payload;
        }
        
        private void OnLoginError(string error)
        {
            if (string.IsNullOrEmpty(error))
                return;
            
            Debug.LogError($"Error: {error}");
            _interactableSignInButton.Value = true;
        }

        private void OnSuccessLogin(Unit unit)
        {
            Debug.Log("Success login");
            _interactableSignInButton.Value = true;
            _screenService.CloseScreen<LobbyLoginScreen>();
            _screenService.CloseScreen<GameRoomHubScreen>();
            _screenService.OpenSync<LobbyScreen>();
        }
        
        private void OnPasswordChanged(string password)
        {
            if (string.IsNullOrEmpty(password))
                return;
            
            _password = password;
        }
        
        private void OnCloseScreenButton(Unit unit)
        {
            _screenService.CloseScreen<LobbyLoginScreen>();
        }
        
        private void OnButtonLogin(Unit unit)
        {
            if (string.IsNullOrEmpty(_password))
                return;
            
            _interactableSignInButton.Value = false;
            
            _lobbyService.JoinLobby(_roomData.Id, _password);
        }
    }
}