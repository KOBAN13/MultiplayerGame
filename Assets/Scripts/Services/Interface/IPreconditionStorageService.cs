using System.Collections.Generic;
using Input;
using Player.Db;

namespace Services.Interface
{
    public interface IPreconditionStorageService
    {
        bool TryFindPreconditionState(out PredictionStateFrame predictionStateFrame, long lastProcessedInputSequence);
        void ClearOldCommands(long lastProcessedInputSequence);
        
        void AddPrecondition(in PredictionStateFrame predictionStateFrame);
        void AddInputFrame(in InputFrame inputFrame);
        int CopyAfterTick(long inputTickExclusive, List<InputFrame> destination);
        int CopyLast(int maxCount, List<InputFrame> destination);
    }
}
