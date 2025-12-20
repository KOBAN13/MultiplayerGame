using Helpers;
using R3;
using Services.Interface;
using UI.Base;
using UI.Helpers;
using UI.View;
using VContainer;

namespace UI.ViewModel
{
    public class NoteViewModel : Base.ViewModel
    {
        [Inject] 
        private IScreenService _screenService;
        
        [Inject] 
        private INoteService _noteScreen;
        
        [AutoBind]
        public readonly ViewModelBinder<string> DiscriptionTextViewBinder = new ();
        
        [AutoBind]
        public readonly ViewModelBinder<string> TitleTextViewBinder = new ();
        
        [AutoBind]
        public readonly RefTypeViewModelBinder<ReactiveCommand> ButtonViewBinder = new ();
        
        public override void Initialize()
        {
            Bind(DiscriptionTextViewBinder, TitleTextViewBinder, ButtonViewBinder);
            
            ButtonViewBinder.Value
                .Subscribe(_ => _screenService.CloseScreen<NoteScreen>())
                .AddTo(Disposable);
            
            _noteScreen.Title.Subscribe(OnNoteTitleChanged).AddTo(Disposable);
            _noteScreen.Description.Subscribe(OnNoteDescriptionChanged).AddTo(Disposable);
        }
        
        private void OnNoteDescriptionChanged(string description)
        {
            DiscriptionTextViewBinder.Value = description;
        }
        
        private void OnNoteTitleChanged(string title)
        {
            TitleTextViewBinder.Value = title;
        }
    }
}