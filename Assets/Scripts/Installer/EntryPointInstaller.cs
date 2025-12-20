using Services.SceneManagement;
using Services.SceneManagement.Enums;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Installer
{
    public class EntryPointInstaller : MonoBehaviour, IInitializable
    {
        [Inject] private SceneLoader _sceneLoader;

        public async void Initialize()
        {
            await _sceneLoader.LoadScene(TypeScene.MainMenu);
        }
    }
}