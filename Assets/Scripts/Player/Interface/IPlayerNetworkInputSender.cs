using Input;

namespace Player.Interface
{
    public interface IPlayerNetworkInputSender
    {
        void SendServerPlayerInput(InputFrame inputFrame);
        void Dispose();
    }
}
