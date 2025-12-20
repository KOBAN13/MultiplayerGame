using System;
using R3;

namespace Services.Interface
{
    public interface ILoginClientService
    {
        bool IsUserLogin();
        void Login(string login, string password);
        ReadOnlyReactiveProperty<string> LoginErrorRequest { get; }
        ReadOnlyReactiveProperty<string> UserName { get; }
        Observable<Unit> SuccessLogin { get; }
    }
}