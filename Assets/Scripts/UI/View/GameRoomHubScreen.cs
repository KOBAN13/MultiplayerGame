using Di;
using Pool;
using UI.Base;
using UI.Binders;
using UI.Helpers;
using UI.ViewModel;
using UnityEngine;
using VContainer;

namespace UI.View
{
    public class GameRoomHubScreen : Screen<GameRoomHubViewModel>
    {
        [SerializeField, AutoBind] private TextViewBinder _userNameBinder;
        [SerializeField, AutoBind] private ButtonViewBinder _createRoomButtonBinder;
        [SerializeField, AutoBind] private ButtonViewBinder _logoutButtonBinder;
        
        [SerializeField] private GameObject _lobbyList;
        
        public override void Initialize()
        {
            Bind<LobbyTimeScope>(
                _userNameBinder,
                _createRoomButtonBinder, 
                _logoutButtonBinder
            );

            ViewModel.SetParentObject.Execute(_lobbyList);
        }

        public override void BindGuid()
        {
            ViewModel.UserNameBinder.AutoBindId.SetGuid(_userNameBinder.AutoBindId.GeneratedGuid);
            ViewModel.CreateRoomButtonBinder.AutoBindId.SetGuid(_createRoomButtonBinder.AutoBindId.GeneratedGuid);
            ViewModel.LogoutButtonBinder.AutoBindId.SetGuid(_logoutButtonBinder.AutoBindId.GeneratedGuid);
        }
    }
}