using Di;
using UI.Base;
using UI.Binders;
using UI.Helpers;
using UI.ViewModel;
using UnityEngine;

namespace UI.View
{
    public class PlayerLobbyItem : View<PlayerLobbyItemViewModel>
    {
        [SerializeField, AutoBind] private ButtonViewBinder _kickPlayerButton;
        [SerializeField, AutoBind] private TextViewBinder _pingPlayerText;
        [SerializeField, AutoBind] private TextViewBinder _userNameText;
        [SerializeField, AutoBind] private GameObjectViewBinder _kickPlayerObject;
        
        public override void Initialize()
        {
            Bind<LobbyTimeScope>(
                _kickPlayerButton,
                _pingPlayerText,
                _userNameText,
                _kickPlayerObject
            );
        }

        public override void BindGuid()
        {
            ViewModel.KickPlayerCommand.AutoBindId.SetGuid(_kickPlayerButton.AutoBindId.GeneratedGuid);
            ViewModel.PingPlayerText.AutoBindId.SetGuid(_pingPlayerText.AutoBindId.GeneratedGuid);
            ViewModel.UserNameText.AutoBindId.SetGuid(_userNameText.AutoBindId.GeneratedGuid);
            ViewModel.KickPlayerObject.AutoBindId.SetGuid(_kickPlayerObject.AutoBindId.GeneratedGuid);
        }
    }
}