namespace Db.Interface
{
    public interface IPredictionParameters
    { 
        float SmallError { get; } 
        float MediumError { get; } 
        float LargeError { get; } 
        float VisualHalfLife { get; } 
        float MediumMoveGain { get; }
        
        int MaxBufferSize { get; }
        int CountGenerateStateSendToServer { get; }
        int CountGenerateStateLocalSimulation { get; }
    }
}