using System.Collections.Generic;
using Player.Db;

namespace Services.Interface
{
    public interface IPreconditionStorageService
    {
        List<PredictionStateFrame> _pendingFrames { get; }
        
        void AddPrecondition(in PredictionStateFrame predictionStateFrame);
        int CopyLast(int maxCount, List<PredictionStateFrame> destination);
    }
}
