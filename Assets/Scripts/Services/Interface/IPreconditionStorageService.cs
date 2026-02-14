using System.Collections.Generic;
using Input;
using Player.Db;
using UnityEngine;

namespace Services.Interface
{
    public interface IPreconditionStorageService
    {
        void Reconciliation(Vector3 position, long lastProcessedInputSequence);
        void AddPrecondition(in PredictionStateFrame predictionStateFrame);
        void AddInputFrame(in InputFrame inputFrame);
        int CopyLast(int maxCount, List<InputFrame> destination);
    }
}
