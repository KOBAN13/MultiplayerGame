using System.Collections.Generic;
using Player.Db;

namespace Services.Interface
{
    public interface IPreconditionStorageService
    {
        void AddPrecondition(in PredictionStateFrame predictionStateFrame);
        int CopyLast(int maxCount, List<PredictionStateFrame> destination);
    }
}
