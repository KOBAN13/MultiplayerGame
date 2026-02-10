using Player.Db;

namespace Player.Interface.Local
{
    public interface IPredictionBuffer
    {
        int Count { get; }
        void Enqueue(in PredictionStateFrame predictionStateFrame);
        bool TryDequeue(out PredictionStateFrame predictionStateFrame);
        void Clear();
    }
}
