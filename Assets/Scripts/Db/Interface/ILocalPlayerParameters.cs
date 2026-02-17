namespace Db.Interface
{
    public interface ILocalPlayerParameters
    {
        float RotationSmoothTime { get; }
        float JumpVelocity { get; }
        float Gravity { get; }
        float SpeedWalk { get; }
        float SpeedRun { get; }
    }
}