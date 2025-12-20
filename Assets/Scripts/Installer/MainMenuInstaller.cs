using Services;
using UI.View;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Installer
{
    public class MainMenuInstaller : MonoBehaviour, IInitializable
    {
        [Inject] private ScreenService _screenService;

        public async void Initialize()
        {
            await _screenService.OpenAsync<MainMenuScreen>();
        }
    }
}