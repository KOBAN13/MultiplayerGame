using Input;
using Player.Db;

namespace Player.Interface.Local
{
    public interface IPredictionStateProvider
    {
        PredictionStateFrame Read(InputFrame inputFrame, ClientStateFrame clientStateFrame);
    }
}