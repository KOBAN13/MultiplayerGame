using R3;

namespace Services.Interface
{
    public interface IPingService
    {
        public Observable<Unit> OnPingUpdate { get; }
    }
}