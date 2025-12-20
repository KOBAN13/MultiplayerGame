using Helpers;
using R3;
using Services;
using Services.Interface;
using UI.Base;
using UI.Helpers;
using UI.View;
using UnityEngine;
using VContainer;

namespace UI.ViewModel
{
    public class ConnectionErrorViewModel : Base.ViewModel
    {
        [Inject]
        private IConnectionService _connectionService;
        
        [Inject] 
        private ScreenService _screenService;
        
        [AutoBind]
        public readonly ViewModelBinder<string> TextViewBinder = new ();
        
        [AutoBind]
        public readonly RefTypeViewModelBinder<ReactiveCommand> ButtonViewBinder = new ();


        public override void Initialize()
        {
            Bind(TextViewBinder, ButtonViewBinder);
            
            ButtonViewBinder.Value
                .Subscribe(OnClickExitGame)
                .AddTo(Disposable);
            
            _connectionService.ConnectionErrorDescription
                .Subscribe(OnConnectionErrorDescriptionChanged)
                .AddTo(Disposable);
        }
        
        private void OnConnectionErrorDescriptionChanged(string description)
        {
            TextViewBinder.Value = "Error description: " + description;
        }

        private void OnClickExitGame(Unit unit)
        {
            _screenService.CloseScreen<ConnectionErrorScreen>();
            
            Application.Quit();
        }
    }
}