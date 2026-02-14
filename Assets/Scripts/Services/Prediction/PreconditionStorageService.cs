using System;
using System.Collections.Generic;
using Db.Interface;
using Input;
using Player.Db;
using Services.Interface;
using UnityEngine;

namespace Services.Prediction
{
    public class PreconditionStorageService : IPreconditionStorageService
    {
        private readonly Dictionary<long, PredictionStateFrame> _pendingFrames = new();
        private readonly Dictionary<long, InputFrame> _inputFrames = new();
        
        private readonly IPredictionParameters  _predictionParameters;

        public PreconditionStorageService(IPredictionParameters predictionParameters)
        {
            _predictionParameters = predictionParameters;
        }
        
        public void AddPrecondition(in PredictionStateFrame predictionStateFrame)
        {
            _pendingFrames.Add(predictionStateFrame.InputTick, predictionStateFrame);

            if (_pendingFrames.Count > _predictionParameters.MaxBufferSize)
                _pendingFrames.Remove(0);
        }

        public void AddInputFrame(in InputFrame inputFrame)
        {
            _inputFrames.Add(inputFrame.InputTick, inputFrame);
            
            if (_inputFrames.Count > _predictionParameters.MaxBufferSize)
                _inputFrames.Remove(0);
        }

        public int CopyLast(int maxCount, List<InputFrame> destination)
        {
            destination.Clear();

            if (maxCount <= 0 || _inputFrames.Count == 0)
                return 0;

            var count = Math.Min(maxCount, _inputFrames.Count);
            var startIndex = _inputFrames.Count - count;

            for (var i = startIndex; i < _inputFrames.Count; i++)
                destination.Add(_inputFrames[i]);

            return count;
        }

        public void Reconciliation(Vector3 position, long lastProcessedInputSequence)
        {
            var isValidKey = _pendingFrames.TryGetValue(lastProcessedInputSequence, out var predictionStateFrame);
            
            if (isValidKey)
                Debug.LogError($"Position in server: {position}. Last processed input sequence: {predictionStateFrame.Position} and predictionId: {predictionStateFrame.InputTick}");
        }
    }
}
