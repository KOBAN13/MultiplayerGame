using Input;

namespace Player.Interface.Local
{
    public interface IPlayerNetworkInputSender
    {
        void SendServerPlayerInput(InputFrame inputFrame);
        void Dispose();
    }
}
