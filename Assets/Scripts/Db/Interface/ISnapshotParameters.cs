namespace Db.Interface
{
    public interface ISnapshotParameters
    {
        int MaxBufferSize { get; }
        float InterpolationBackTime { get; }
    }
}