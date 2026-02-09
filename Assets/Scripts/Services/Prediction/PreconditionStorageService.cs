using System.Collections.Generic;
using Db.Interface;
using Player.Db;
using Services.Interface;

namespace Services.Prediction
{
    public class PreconditionStorageService : IPreconditionStorageService
    {
        private readonly List<PredictionStateFrame> _pendingFrames = new();
        
        private readonly IPredictionParameters  _predictionParameters;

        public PreconditionStorageService(IPredictionParameters predictionParameters)
        {
            _predictionParameters = predictionParameters;
        }
        
        public void AddPrecondition(in PredictionStateFrame predictionStateFrame)
        {
            _pendingFrames.Add(predictionStateFrame);

            if (_pendingFrames.Count > _predictionParameters.MaxBufferSize)
                _pendingFrames.RemoveAt(0);
        }
    }
}