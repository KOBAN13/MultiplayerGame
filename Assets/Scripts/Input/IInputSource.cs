using R3;

namespace Input
{
    public interface IInputSource
    {
        ReactiveCommand<bool> AimCommand { get; }
        ReactiveCommand<bool> ShotCommand { get; }
        InputFrame Read();
    }
}
