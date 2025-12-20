using Db;
using R3;
using Services.Interface;
using UI.Base;
using UI.Helpers;
using UI.Utils;
using UI.View;
using VContainer;

namespace UI.ViewModel
{
    public class GameListItemViewModel : Base.ViewModel
    {
        [Inject] private ILobbyService _lobbyService;
        [Inject] private IScreenService _screenService;
        
        [AutoBind]
        public readonly RefTypeViewModelBinder<ReactiveCommand> PlayButtonBinder = new ();
        
        [AutoBind]
        public readonly ViewModelBinder<string> PlayerInLobbyBinder = new ();
        
        [AutoBind]
        public readonly ViewModelBinder<string> RoomNameBinder = new ();
        
        [AutoBind]
        public readonly ViewModelBinder<EUIObjectState> LockIconBinder = new();
        
        private readonly ReactiveProperty<bool> _interactablePlayButton = new(true);
        
        public ReactiveProperty<bool> InteractablePlayButton => _interactablePlayButton;

        public RoomData RoomData { get; private set; }

        public override void Initialize()
        {
            Bind(PlayButtonBinder, PlayerInLobbyBinder, RoomNameBinder, LockIconBinder);
            
            PlayButtonBinder.Value.Subscribe(OnPlayButton).AddTo(Disposable);
        }

        public void UpdateGameListItem(RoomData room)
        {
            var playerSlots = room.MaxUsers - room.UserCount;
            
            _interactablePlayButton.Value = playerSlots > 0;
            
            RoomNameBinder.Value = room.Name;
            PlayerInLobbyBinder.Value = $"Player slots: {playerSlots}";
            
            RoomData = room;
            UpdateLockIcon();
        }
        
        private void UpdateLockIcon()
        {
            //TODO: Удалить Room data, не нужна вещь
            LockIconBinder.Value = RoomData.IsPrivate 
                ? EUIObjectState.Show 
                : EUIObjectState.Hide;
        }
        
        private void OnPlayButton(Unit unit)
        {
            if (RoomData.IsPrivate)
            {
                _screenService.OpenSync<LobbyLoginScreen, RoomData>(RoomData);
            }
            else
            {
                _lobbyService.JoinLobby(RoomData.Id);
                _screenService.OpenSync<LobbyScreen>();
            }
        }
    }
}