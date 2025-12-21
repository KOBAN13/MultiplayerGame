namespace Player.Interface
{
    public interface IPlayerNetworkInputSender
    {
        void SendServerPlayerInput();
        void Dispose();
    }
}