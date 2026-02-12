namespace Db.Interface
{
    public interface IPredictionParameters
    {
        int MaxBufferSize { get; }
        int CountGenerateStateSendToServer { get; }
        int CountGenerateStateLocalSimulation { get; }
    }
}