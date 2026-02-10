namespace Db.Interface
{
    public interface IPredictionParameters
    {
        int MaxBufferSize { get; }
        int CountGenerateStateInSeconds { get; }
    }
}