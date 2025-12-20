using Manager.Interface;
using R3;
using Services.Interface;
using Sfs2X.Entities;
using UI.Base;
using UI.Utils;
using VContainer;

namespace UI.ViewModel
{
    public class PlayerLobbyItemViewModel : Base.ViewModel
    {
        [Inject] private ILobbyService _lobbyService;
        [Inject] private ISessionManager _sessionManager;
        
        public readonly RefTypeViewModelBinder<ReactiveCommand> KickPlayerCommand = new();
        public readonly ViewModelBinder<EUIObjectState> KickPlayerObject = new();
        public readonly ViewModelBinder<string> PingPlayerText = new();
        public readonly ViewModelBinder<string> UserNameText = new();
        
        public override void Initialize()
        {
            Bind(KickPlayerCommand, PingPlayerText, UserNameText, KickPlayerObject);

            KickPlayerCommand.Value.Subscribe(OnKickPlayerInLobby).AddTo(Disposable);
        }
        
        public void UpdateGameListItem(string userName, string ping)
        {
            UserNameText.Value = userName;
            PingPlayerText.Value = ping;
        }
        
        public void ActivityKickPlayerButton(EUIObjectState state)
        {
            KickPlayerObject.Value = state;
        }
        
        private void OnKickPlayerInLobby(Unit unit)
        {
            var userId = _sessionManager.FindUserIdByName(UserNameText.Value);
            
            _lobbyService.KickUser(userId);
        }
    }
}