using Di;
using UI.Base;
using UI.Binders;
using UI.Helpers;
using UI.ViewModel;
using UnityEngine;

namespace UI.View
{
    public class LobbyScreen : Screen<LobbyViewModel>
    {
        [SerializeField, AutoBind] private ButtonViewBinder _startGameButtonBinder;
        [SerializeField, AutoBind] private ButtonViewBinder _inviteGameButtonBinder;
        [SerializeField, AutoBind] private ButtonViewBinder _leaveGameButtonBinder;
        [SerializeField, AutoBind] private GameObjectViewBinder _objectStartGameButtonBinder;
        
        [SerializeField] private GameObject _lobbyListContent;
        
        public override void Initialize()
        {
            Bind<LobbyTimeScope>(
                _startGameButtonBinder, 
                _inviteGameButtonBinder, 
                _leaveGameButtonBinder,
                _objectStartGameButtonBinder
            );
            
            ViewModel.SetParentObject.Execute(_lobbyListContent);
        }

        public override void BindGuid()
        {
            ViewModel.InvitePlayerCommand.AutoBindId.SetGuid(_inviteGameButtonBinder.AutoBindId.GeneratedGuid);
            ViewModel.StartGameCommand.AutoBindId.SetGuid(_startGameButtonBinder.AutoBindId.GeneratedGuid);
            ViewModel.LeaveGameCommand.AutoBindId.SetGuid(_leaveGameButtonBinder.AutoBindId.GeneratedGuid);
            ViewModel.ObjectStartGameCommand.AutoBindId.SetGuid(_objectStartGameButtonBinder.AutoBindId.GeneratedGuid);
        }
    }
}