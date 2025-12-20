using R3;

namespace Services.Interface
{
    public interface INoteService
    {
        Observable<string> Title { get; }
        Observable<string> Description { get; }
        
        void ClearNote();
        void UpdateNote(string title, string description);
        void SetTitle(string title);
        void SetDescription(string description);
    }
}