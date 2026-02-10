using System.Collections.Generic;
using Db.Interface;
using Player.Db;
using Player.Interface.Local;

namespace Player.Prediction
{
    public class PredictionBuffer : IPredictionBuffer
    {
        private readonly Queue<PredictionStateFrame> _buffer = new();
        private readonly IPredictionParameters _predictionParameters;

        public PredictionBuffer(IPredictionParameters predictionParameters)
        {
            _predictionParameters = predictionParameters;
        }

        public int Count => _buffer.Count;

        public void Enqueue(in PredictionStateFrame predictionStateFrame)
        {
            _buffer.Enqueue(predictionStateFrame);
            Trim();
        }

        public bool TryDequeue(out PredictionStateFrame predictionStateFrame)
        {
            if (_buffer.Count == 0)
            {
                predictionStateFrame = default;
                return false;
            }

            predictionStateFrame = _buffer.Dequeue();
            return true;
        }

        public void Clear()
        {
            _buffer.Clear();
        }

        private void Trim()
        {
            var maxSize = _predictionParameters.MaxBufferSize;
            
            if (maxSize <= 0)
            {
                _buffer.Clear();
                return;
            }

            while (_buffer.Count > maxSize)
                _buffer.Dequeue();
        }
    }
}
