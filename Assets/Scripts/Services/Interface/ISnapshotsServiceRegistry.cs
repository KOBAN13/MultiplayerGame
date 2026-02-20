namespace Services.Interface
{
    public interface ISnapshotsServiceRegistry
    {
        void AddSnapshotService(int playerId);
        ISnapshotsService GetSnapshotService(int playerId);
        bool RemoveSnapshotService(int playerId);
        void Clear();
    }
}
