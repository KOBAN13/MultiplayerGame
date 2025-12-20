using System;
using R3;
using Services.Interface;

namespace Services
{
    public class NoteService : INoteService
    {
        private readonly ReactiveProperty<string> _title = new();
        private readonly ReactiveProperty<string> _description = new();
        
        public Observable<string> Title => _title;
        public Observable<string> Description => _description;
        
        public void ClearNote()
        {
            _title.Value = string.Empty;
            _description.Value = string.Empty;
        }
        
        public void UpdateNote(string title, string description)
        {
            _title.Value = title;
            _description.Value = description;
        }
        
        public void SetTitle(string title)
        {
            _title.Value = title;
        }
        
        public void SetDescription(string description)
        {
            _description.Value = description;
        }
    }
}