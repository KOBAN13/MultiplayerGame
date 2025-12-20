using System.Collections.Generic;
using Db;
using ObservableCollections;
using Pool;
using R3;
using Services.Interface;
using UI.Base;
using UI.View;
using UnityEngine;
using VContainer;

namespace UI.ViewModel
{
    public class GameRoomHubViewModel : Base.ViewModel
    {
        [Inject] private IGameHubService _gameHubService;
        [Inject] private ILoginClientService _loginClientService;
        [Inject] private IScreenService _screenService;
        [Inject] private IGameListItemPool _gameListItemPool;

        public readonly ViewModelBinder<string> UserNameBinder = new();
        public readonly RefTypeViewModelBinder<ReactiveCommand> CreateRoomButtonBinder = new();
        public readonly RefTypeViewModelBinder<ReactiveCommand> LogoutButtonBinder = new();
        public readonly ReactiveCommand<GameObject> SetParentObject = new();

        public override void Initialize()
        {
            SetParentObject.Subscribe(InitUI).AddTo(Disposable);
            
            Bind(UserNameBinder, CreateRoomButtonBinder, LogoutButtonBinder);
            UserNameBinder.Value = _loginClientService.UserName.CurrentValue;

            CreateRoomButtonBinder.Value.Subscribe(_ => OnCreateRoom()).AddTo(Disposable);
            LogoutButtonBinder.Value.Subscribe(_ => OnLogout()).AddTo(Disposable);
            
            _gameHubService.Rooms.ObserveAdd().Subscribe(kvp => OnRoomAdded(kvp.Value)).AddTo(Disposable);
            _gameHubService.Rooms.ObserveRemove().Subscribe(kvp => OnRoomRemoved(kvp.Value)).AddTo(Disposable);
            _gameHubService.Rooms.ObserveReplace().Subscribe(kvp => OnRoomUpdated(kvp.NewValue)).AddTo(Disposable);
        }

        private void InitUI(GameObject parent)
        {
            _gameListItemPool.Initialize(parent);
        }

        private void OnCreateRoom() => _screenService.OpenSync<CreateRoomScreen>();

        private void OnLogout()
        {
            
        }
        
        private void OnRoomAdded(KeyValuePair<int, RoomData> kvp)
        {
            var item = _gameListItemPool.GetListItem(kvp.Key);
            
            item.ViewModel.UpdateGameListItem(kvp.Value);
        }

        private void OnRoomRemoved(KeyValuePair<int, RoomData> kvp)
        {
            _gameListItemPool.ReleaseListItem(kvp.Key);
        }

        private void OnRoomUpdated(KeyValuePair<int, RoomData> kvp)
        {
            var item = _gameListItemPool.GetById(kvp.Key);
            item.ViewModel.UpdateGameListItem(kvp.Value);
        }
    }
}
