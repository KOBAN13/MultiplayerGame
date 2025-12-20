using Cysharp.Threading.Tasks;
using UI.Base;
using VContainer;

namespace Services.Interface
{
    public interface IScreenService
    {
        UniTask<TScreen> OpenAsync<TScreen>() where TScreen : View;
        UniTask<TScreen> OpenAsync<TScreen, TPayload>(TPayload payload) where TScreen : View;
        TScreen OpenSync<TScreen>() where TScreen : View;
        TScreen OpenSync<TScreen, TPayload>(TPayload payload) where TScreen : View;
        void ClearCollections();
        void CloseScreen<TScreen>() where TScreen : View;
        void Close();
    }
}