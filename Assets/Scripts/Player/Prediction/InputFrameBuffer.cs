using System.Collections.Generic;
using Db.Interface;
using Input;
using Player.Interface.Local;

namespace Player.Prediction
{
    public class InputFrameBuffer : IInputFrameBuffer
    {
        private readonly Queue<InputFrame> _buffer = new();
        
        private readonly IPredictionParameters _predictionParameters;

        public InputFrameBuffer(IPredictionParameters predictionParameters)
        {
            _predictionParameters = predictionParameters;
        }

        public int Count => _buffer.Count;

        public void Enqueue(in InputFrame predictionStateFrame)
        {
            _buffer.Enqueue(predictionStateFrame);
            Trim();
        }

        public bool TryDequeue(out InputFrame predictionStateFrame)
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
