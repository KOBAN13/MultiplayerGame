using Di;
using R3;
using UI.Base;
using UI.Binders;
using UI.Helpers;
using UI.ViewModel;
using UnityEngine;

namespace UI.View
{
    public class CreateRoomScreen : Screen<CreateRoomViewModel>
    {
        [SerializeField, AutoBind] private ToggleViewBinder _isPrivateRoomBinder;
        [SerializeField, AutoBind] private InputFieldTextChangedViewBinder _roomNameTextViewBinder;
        [SerializeField, AutoBind] private InputFieldTextChangedViewBinder _roomPasswordTextViewBinder;
        [SerializeField, AutoBind] private InputFieldTextChangedViewBinder _roomMaxPlayersTextViewBinder;
        [SerializeField, AutoBind] private ButtonViewBinder _createRoomButtonBinder;
        [SerializeField, AutoBind] private GameObjectViewBinder _passwordObjectBinder;
        [SerializeField, AutoBind] private ButtonViewBinder _closeScreenButtonBinder;
        
        public override void Initialize()
        {
            Bind<LobbyTimeScope>(
                _isPrivateRoomBinder, 
                _roomNameTextViewBinder,
                _roomPasswordTextViewBinder, 
                _createRoomButtonBinder,
                _roomMaxPlayersTextViewBinder,
                _passwordObjectBinder,
                _closeScreenButtonBinder
            );
            
            BindGuid();
            
            ViewModel.InteractableCreateRoomButton.Subscribe(InteractableCreateRoomButton).AddTo(this);
        }

        public override void BindGuid()
        {
            ViewModel.IsPrivateRoomBinder.AutoBindId.SetGuid(_isPrivateRoomBinder.AutoBindId.GeneratedGuid);
            ViewModel.RoomNameTextViewBinder.AutoBindId.SetGuid(_roomNameTextViewBinder.AutoBindId.GeneratedGuid);
            ViewModel.CreateRoomButtonBinder.AutoBindId.SetGuid(_createRoomButtonBinder.AutoBindId.GeneratedGuid);
            ViewModel.PasswordTextViewBinder.AutoBindId.SetGuid(_roomPasswordTextViewBinder.AutoBindId.GeneratedGuid);
            ViewModel.MaxPlayersTextViewBinder.AutoBindId.SetGuid(_roomMaxPlayersTextViewBinder.AutoBindId.GeneratedGuid);
            ViewModel.PasswordObjectBinder.AutoBindId.SetGuid(_passwordObjectBinder.AutoBindId.GeneratedGuid);
            ViewModel.CloseScreenButtonBinder.AutoBindId.SetGuid(_closeScreenButtonBinder.AutoBindId.GeneratedGuid);
        }
        
        private void InteractableCreateRoomButton(bool interactable)
        {
            _createRoomButtonBinder.Button.interactable = interactable;
        }
    }
}