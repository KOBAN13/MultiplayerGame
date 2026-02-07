namespace Db.Interface
{
    public interface ISnapshotParameters
    {
        int MaxBufferSize { get; }
        float InterpolationBackTime { get; }
        bool UseAdaptiveBackTime { get; }
        float AdaptiveBackTimeMin { get; }
        float AdaptiveBackTimeMax { get; }
        float JitterMultiplier { get; }
        float JitterSmoothing { get; }
        float DebugLogInterval { get; }
        bool EnableInterpolationDebug { get; }
    }
}
