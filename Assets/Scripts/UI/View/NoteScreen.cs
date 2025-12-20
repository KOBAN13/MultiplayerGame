using Di;
using Helpers;
using UI.Base;
using UI.Binders;
using UI.Helpers;
using UI.ViewModel;
using UnityEngine;

namespace UI.View
{
    public class NoteScreen : Screen<NoteViewModel>
    {
        [SerializeField, AutoBind] private TextViewBinder _discriptionTextViewBinder = new();
        [SerializeField, AutoBind] private TextViewBinder _titleTextViewBinder = new();
        [SerializeField, AutoBind] private ButtonViewBinder _buttonViewBinder = new();
        
        public override void Initialize()
        {
            Bind<MainMenuTimeScope>(_discriptionTextViewBinder, _titleTextViewBinder, _buttonViewBinder);
        }

        public override void BindGuid()
        {
            ViewModel.DiscriptionTextViewBinder.AutoBindId.SetGuid(_discriptionTextViewBinder.AutoBindId.GeneratedGuid);
            ViewModel.TitleTextViewBinder.AutoBindId.SetGuid(_titleTextViewBinder.AutoBindId.GeneratedGuid);
            ViewModel.ButtonViewBinder.AutoBindId.SetGuid(_buttonViewBinder.AutoBindId.GeneratedGuid);
        }
    }
}