using R3;
using Services;
using Services.Interface;
using UI.Base;
using UI.Utils;
using UI.View;
using VContainer;

namespace UI.ViewModel
{
    public class CreateRoomViewModel : Base.ViewModel
    {
        [Inject] private IGameHubService _gameHubService;
        [Inject] private IScreenService _screenService;
        [Inject] private ILobbyService _lobbyService;
        
        public readonly RefTypeViewModelBinder<ReactiveCommand<bool>> IsPrivateRoomBinder = new();
        public readonly RefTypeViewModelBinder<ReactiveCommand> CreateRoomButtonBinder = new();
        public readonly RefTypeViewModelBinder<ReactiveCommand<string>> RoomNameTextViewBinder = new();
        public readonly RefTypeViewModelBinder<ReactiveCommand<string>> PasswordTextViewBinder = new();
        public readonly RefTypeViewModelBinder<ReactiveCommand<string>> MaxPlayersTextViewBinder = new();
        public readonly RefTypeViewModelBinder<ReactiveCommand> CloseScreenButtonBinder = new();
        public readonly ViewModelBinder<EUIObjectState> PasswordObjectBinder = new();
        
        private readonly ReactiveProperty<bool> _interactableCreateRoomButton = new(true);
        
        public Observable<bool> InteractableCreateRoomButton => _interactableCreateRoomButton;

        private bool _isPrivateRoom;
        private short _maxPlayers;
        private string _roomName;
        private string _password = string.Empty;
        
        public override void Initialize()
        {
            Bind(
                IsPrivateRoomBinder,
                CreateRoomButtonBinder,
                RoomNameTextViewBinder,
                PasswordTextViewBinder,
                PasswordObjectBinder,
                MaxPlayersTextViewBinder,
                CloseScreenButtonBinder
            );

            CreateRoomButtonBinder.Value.Subscribe(OnCreateRoom).AddTo(Disposable);
            IsPrivateRoomBinder.Value.Subscribe(OnIsPrivateRoomChanged).AddTo(Disposable);
            RoomNameTextViewBinder.Value.Subscribe(OnRoomNameChanged).AddTo(Disposable);
            PasswordTextViewBinder.Value.Subscribe(OnPasswordChanged).AddTo(Disposable);
            MaxPlayersTextViewBinder.Value.Subscribe(OnMaxPlayersChanged).AddTo(Disposable);
            CloseScreenButtonBinder.Value.Subscribe(OnCloseScreen).AddTo(Disposable);
            _gameHubService.CreateRoomCommand.Subscribe(OnOpenLobbyScreen).AddTo(Disposable);
        }
        
        private void OnCreateRoom(Unit unit)
        {
            _gameHubService.CreateRoom(_roomName, _maxPlayers, _isPrivateRoom, _password);
            
            _interactableCreateRoomButton.Value = false;
        }
        
        private void OnIsPrivateRoomChanged(bool isPrivateRoom)
        {
            _isPrivateRoom = isPrivateRoom;
            
            PasswordObjectBinder.Value = isPrivateRoom ? EUIObjectState.Show : EUIObjectState.Hide;
        }
        
        private void OnRoomNameChanged(string roomName)
        {
            _roomName = roomName;
        }
        
        private void OnPasswordChanged(string password)
        {
            _password = password;
        }
        
        private void OnMaxPlayersChanged(string maxPlayers)
        {
            _maxPlayers = short.Parse(maxPlayers);
        }
        
        private void OnCloseScreen(Unit unit)
        {
            _screenService.CloseScreen<CreateRoomScreen>();
        }
        
        private void OnOpenLobbyScreen(Unit unit)
        {
            _screenService.CloseScreen<CreateRoomScreen>();
            _screenService.CloseScreen<GameRoomHubScreen>();
            _screenService.OpenSync<LobbyScreen>();
        }
    }
}