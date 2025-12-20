using System.Collections.Generic;
using System.Text;
using Db;
using Helpers;
using Manager.Interface;
using ObservableCollections;
using Pool;
using R3;
using Services.Interface;
using Sfs2X.Entities;
using UI.Base;
using UI.Utils;
using UI.View;
using UnityEngine;
using Utils;
using VContainer;

namespace UI.ViewModel
{
    public class LobbyViewModel : Base.ViewModel
    {
        [Inject] private ILobbyService _lobbyService;
        [Inject] private IScreenService _screenService;
        [Inject] private IPlayerLobbyItemPool _playerLobbyItemPool;
        [Inject] private ISessionManager _sessionManager;
        
        public readonly RefTypeViewModelBinder<ReactiveCommand> InvitePlayerCommand = new();
        public readonly RefTypeViewModelBinder<ReactiveCommand> StartGameCommand = new();
        public readonly RefTypeViewModelBinder<ReactiveCommand> LeaveGameCommand = new();
        public readonly ViewModelBinder<EUIObjectState> ObjectStartGameCommand = new();
        
        public readonly ReactiveCommand<GameObject> SetParentObject = new();
        
        private readonly StringBuilder _pingBuilder = new();

        public override void Initialize()
        {
            SetParentObject.Subscribe(InitializeLobby).AddTo(Disposable);
            
            Bind(InvitePlayerCommand, StartGameCommand, LeaveGameCommand, ObjectStartGameCommand);
            
            InvitePlayerCommand.Value.Subscribe(OnInvitePlayer).AddTo(Disposable);
            StartGameCommand.Value.Subscribe(OnStartGame).AddTo(Disposable);
            LeaveGameCommand.Value.Subscribe(OnLeaveGame).AddTo(Disposable);
            
            _lobbyService.KickedUser.Subscribe(OnLeaveGame).AddTo(Disposable);
            
            _lobbyService.RoleChanged.Subscribe(OnDisableStartGameButton).AddTo(Disposable);
            
            _lobbyService.Users.ObserveAdd().Subscribe(kvp => OnUserAdded(kvp.Value)).AddTo(Disposable);
            _lobbyService.Users.ObserveRemove().Subscribe(kvp => OnUserRemoved(kvp.Value)).AddTo(Disposable);
            _lobbyService.Users.ObserveReplace().Subscribe(kvp => OnUserUpdated(kvp.NewValue)).AddTo(Disposable);
        }
        
        private void OnInvitePlayer(Unit unit)
        {
            
        }
        
        private void OnStartGame(Unit unit)
        {
            _lobbyService.StartGame();
        }

        private void OnDisableStartGameButton(Unit unit)
        {
            var role = _sessionManager.GetRole();

            ObjectStartGameCommand.Value = role switch
            {
                ERoomRole.OWNER => EUIObjectState.Show,
                ERoomRole.PLAYER => EUIObjectState.Hide,
                _ => ObjectStartGameCommand.Value
            };
        }
        
        private void OnLeaveGame(Unit unit)
        {
            _lobbyService.LeaveLobby();
            _screenService.CloseScreen<LobbyScreen>();
            _screenService.OpenSync<GameRoomHubScreen>();
        }
        
        private void InitializeLobby(GameObject obj)
        {
            _playerLobbyItemPool.Initialize(obj);
        }
        
        private void OnUserAdded(KeyValuePair<int, User> kvp)
        {
            var item = _playerLobbyItemPool.GetListItem(kvp.Key);
            var ping = kvp.Value.GetVariable(SFSResponseHelper.USER_PING);
            var user = kvp.Value;
            
            _pingBuilder.Clear();
            _pingBuilder.AppendFormat("{0} ms", ping.Value);
            
            item.ViewModel.UpdateGameListItem(user.Name, _pingBuilder.ToString());
            
            var role = _sessionManager.GetRole();
            
            var state = role == ERoomRole.OWNER
                ? EUIObjectState.Show
                : EUIObjectState.Hide;
            
            item.ViewModel.ActivityKickPlayerButton(state);
        }
        
        private void OnUserRemoved(KeyValuePair<int, User> kvp)
        {
            _playerLobbyItemPool.ReleaseListItem(kvp.Key);
        }
        
        private void OnUserUpdated(KeyValuePair<int, User> kvp)
        {
            var item = _playerLobbyItemPool.GetById(kvp.Key);
            var ping = kvp.Value.GetVariable(SFSResponseHelper.USER_PING);
            
            _pingBuilder.Clear();
            _pingBuilder.AppendFormat("{0} ms", ping.Value);
            
            item.ViewModel.UpdateGameListItem(kvp.Value.Name, _pingBuilder.ToString());
            
            var role = _sessionManager.GetRole();
            
            var state = role == ERoomRole.OWNER
                ? EUIObjectState.Show
                : EUIObjectState.Hide;
            
            item.ViewModel.ActivityKickPlayerButton(state);
        }
    }
}