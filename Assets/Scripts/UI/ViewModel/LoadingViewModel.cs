using Helpers;
using Services.SceneManagement;
using UI.Base;
using R3;
using UI.Helpers;
using VContainer;

namespace UI.ViewModel
{
    public class LoadingViewModel : Base.ViewModel
    {
        [Inject] 
        private SceneLoader _sceneLoader;
        
        [AutoBind]
        public readonly ViewModelBinder<float> ProgressBinder = new();
        
        public override void Initialize()
        {
            Bind(ProgressBinder);
            
            _sceneLoader.Progress.Skip(1)
                .Subscribe(value => ProgressBinder.Value = value)
                .AddTo(Disposable);
        }
    }
}