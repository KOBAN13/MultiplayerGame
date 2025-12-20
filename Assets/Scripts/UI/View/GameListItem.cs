using Di;
using R3;
using UI.Base;
using UI.Binders;
using UI.Helpers;
using UI.ViewModel;
using UnityEngine;

namespace UI.View
{
    public class GameListItem : View<GameListItemViewModel>
    {
        [SerializeField, AutoBind] 
        private ButtonViewBinder _playButtonBinder = new();

        [SerializeField, AutoBind] 
        private TextViewBinder _countPlayerInLobbyBinder = new();

        [SerializeField, AutoBind] 
        private TextViewBinder _roomName = new();
        
        [SerializeField, AutoBind]
        private GameObjectViewBinder _lockIcon = new();
        
        public override void Initialize()
        {
            Bind<LobbyTimeScope>(_playButtonBinder, _countPlayerInLobbyBinder, _roomName, _lockIcon);
            
            ViewModel.InteractablePlayButton.Subscribe(OnInteractablePlayButton).AddTo(this);
        }

        public override void BindGuid()
        {
            ViewModel.PlayButtonBinder.AutoBindId.SetGuid(_playButtonBinder.AutoBindId.GeneratedGuid);
            ViewModel.PlayerInLobbyBinder.AutoBindId.SetGuid(_countPlayerInLobbyBinder.AutoBindId.GeneratedGuid);
            ViewModel.RoomNameBinder.AutoBindId.SetGuid(_roomName.AutoBindId.GeneratedGuid);
            ViewModel.LockIconBinder.AutoBindId.SetGuid(_lockIcon.AutoBindId.GeneratedGuid);
        }

        private void OnInteractablePlayButton(bool isInteractable)
        {
            _playButtonBinder.Button.interactable = isInteractable;
        }
    }
}