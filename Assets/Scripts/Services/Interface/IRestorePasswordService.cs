using R3;

namespace Services.Interface
{
    public interface IRestorePasswordService
    {
        void RestorePassword(string email);
        ReadOnlyReactiveProperty<string> RestoreResult { get; }
    }
}