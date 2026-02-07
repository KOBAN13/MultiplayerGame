namespace Services.Interface
{
    public interface IServerTimeService
    {
        void SyncServerTime(float serverTime);
        float GetServerTime();
        bool HasServerTime { get; }
    }
}
