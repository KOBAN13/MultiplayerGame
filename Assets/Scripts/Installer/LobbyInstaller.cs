using Services;
using Services.Interface;
using UI.View;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Installer
{
    public class LobbyInstaller : MonoBehaviour, IInitializable
    {
        [Inject] private ScreenService _screenService;
        [Inject] private IGameHubService _gameHubService;
        [Inject] private ILobbyService _lobbyService;
        
        public async void Initialize()
        {
            await _screenService.OpenAsync<GameRoomHubScreen>();
            
            var lobbyScreen = await _screenService.OpenAsync<LobbyScreen>();
            lobbyScreen.gameObject.SetActive(false);
            
            var createRoomScreen = await _screenService.OpenAsync<CreateRoomScreen>();
            createRoomScreen.gameObject.SetActive(false);
            
            var lobbyLoginScreen = await _screenService.OpenAsync<LobbyLoginScreen>();
            lobbyLoginScreen.gameObject.SetActive(false);
            
            _gameHubService.Initialize();
            _lobbyService.Initialize();
        }
    }
}