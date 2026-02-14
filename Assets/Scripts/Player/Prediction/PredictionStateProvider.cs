using Player.Db;
using Player.Interface.Local;
using UnityEngine;

namespace Player.Prediction
{
    public class PredictionStateProvider : IPredictionStateProvider
    {
        private long _inputTick;
        
        public PredictionStateFrame Read(Vector3 position)
        {
            if (_inputTick == long.MaxValue)
                _inputTick = 0;
            
            _inputTick++;
            
            return new PredictionStateFrame()
            {
                InputTick = _inputTick,
                Position = position,
            };
            
        }
    }
}