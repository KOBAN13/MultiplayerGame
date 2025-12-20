using R3;

namespace Services.Interface
{
    public interface IConnectionService
    {
        ReadOnlyReactiveProperty<string> ConnectionErrorDescription { get; }
    }
}