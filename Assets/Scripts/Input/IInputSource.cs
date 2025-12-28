using R3;

namespace Input
{
    public interface IInputSource
    {
        ReactiveCommand<bool> AimCommand { get; }
        ReactiveCommand ShotCommand { get; }
        InputFrame Read();
    }
}
