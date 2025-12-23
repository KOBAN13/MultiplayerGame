using Input;
using Player.Interface;
using VContainer.Unity;

namespace Player
{
    public class LocalPlayerController : ITickable
    {
        private readonly IInputSource _inputSource;
        private readonly IPlayerNetworkInputSender _playerNetworkInputSender;
        private InputFrame _lastInputFrame;

        public LocalPlayerController(IInputSource inputSource, IPlayerNetworkInputSender playerNetworkInputSender)
        {
            _inputSource = inputSource;
            _playerNetworkInputSender = playerNetworkInputSender;
        }

        public void Tick()
        {
            _lastInputFrame = _inputSource.Read();
            _playerNetworkInputSender.SendServerPlayerInput(_lastInputFrame);
        }
    }
}