using Input;
using Player.Db;

namespace Player.Interface.Local
{
    public interface IPlayerNetworkStateSender
    {
        void SendServerPlayerState(InputFrame inputFrame);
        void SendServerPlayerInput(ClientStateFrame stateFrame);
    }
}
