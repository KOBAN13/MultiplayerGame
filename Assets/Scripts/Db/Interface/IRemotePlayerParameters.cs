namespace Db.Interface
{
    public interface IRemotePlayerParameters
    {
        float VisualPositionSmoothTime { get; }
        float VisualRotationLerpSpeed { get; }
        float VisualSnapDistance { get; }
        float RotationSmoothTime { get; }
    }
}