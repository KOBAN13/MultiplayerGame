using System;
using System.Collections.Generic;
using Db.Interface;
using Player.Db;
using Services.Interface;

namespace Services.Prediction
{
    public class PreconditionStorageService : IPreconditionStorageService
    {
        public List<PredictionStateFrame> _pendingFrames { get; } = new List<PredictionStateFrame>();
        
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

        public int CopyLast(int maxCount, List<PredictionStateFrame> destination)
        {
            destination.Clear();

            if (maxCount <= 0 || _pendingFrames.Count == 0)
                return 0;

            var count = Math.Min(maxCount, _pendingFrames.Count);
            var startIndex = _pendingFrames.Count - count;

            for (var i = startIndex; i < _pendingFrames.Count; i++)
                destination.Add(_pendingFrames[i]);

            return count;
        }
    }
}
