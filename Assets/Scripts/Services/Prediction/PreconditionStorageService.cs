using System;
using System.Collections.Generic;
using Db.Interface;
using Input;
using Player.Db;
using Services.Interface;

namespace Services.Prediction
{
    public class PreconditionStorageService : IPreconditionStorageService
    {
        private readonly List<PredictionStateFrame> _pendingFrames = new();
        private readonly List<InputFrame> _inputFrames = new();
        
        private readonly IPredictionParameters  _predictionParameters;

        public PreconditionStorageService(IPredictionParameters predictionParameters)
        {
            _predictionParameters = predictionParameters;
        }

        public bool TryFindPreconditionState(out PredictionStateFrame predictionStateFrame, long lastProcessedInputSequence)
        {
            for (var i = _pendingFrames.Count - 1; i >= 0; i--)
            {
                predictionStateFrame = _pendingFrames[i];
                
                if (predictionStateFrame.InputTick != lastProcessedInputSequence)
                    continue;
                
                return true;
            }
            
            predictionStateFrame = default;
            return false;
        }

        public void ClearOldCommands(long lastProcessedInputSequence)
        {
            if (lastProcessedInputSequence <= 0)
                return;
            
            _pendingFrames.RemoveAll(frame => frame.InputTick <= lastProcessedInputSequence);
            _inputFrames.RemoveAll(frame => frame.InputTick <= lastProcessedInputSequence);
        }
        
        public void AddPrecondition(in PredictionStateFrame predictionStateFrame)
        {
            var inputTick = predictionStateFrame.InputTick;
            
            var index = _pendingFrames.FindIndex(frame => frame.InputTick == inputTick);

            if (index >= 0)
                _pendingFrames[index] = predictionStateFrame;
            else
                _pendingFrames.Add(predictionStateFrame);

            if (_pendingFrames.Count > _predictionParameters.MaxBufferSize)
                _pendingFrames.RemoveAt(0);
        }

        public void AddInputFrame(in InputFrame inputFrame)
        {
            _inputFrames.Add(inputFrame);
            
            if (_inputFrames.Count > _predictionParameters.MaxBufferSize)
                _inputFrames.RemoveAt(0);
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

        public int CopyAfterTick(long inputTickExclusive, List<InputFrame> destination)
        {
            destination.Clear();

            if (_inputFrames.Count == 0)
                return 0;

            foreach (var inputFrame in _inputFrames)
            {
                if (inputFrame.InputTick <= inputTickExclusive)
                    continue;

                destination.Add(inputFrame);
            }

            return destination.Count;
        }
    }
}
