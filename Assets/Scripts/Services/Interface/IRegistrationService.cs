using R3;

namespace Services.Interface
{
    public interface IRegistrationService
    {
        ReadOnlyReactiveProperty<string> RegisterError { get; }
        void Register(string email, string login, string password);
        Observable<Unit> SuccessRegistration { get; }
    }
}