namespace Db.Interface
{
    public interface IRotationCameraParameters
    {
        float Sensitivity { get; }
        float TopClamp { get; }
        float BottomClamp { get; }
        float AngleOverride { get; }
    }
}