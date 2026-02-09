using Player.Db;

namespace Services.Interface
{
    public interface IPreconditionStorageService
    {
        void AddPrecondition(in PredictionStateFrame predictionStateFrame);
    }
}