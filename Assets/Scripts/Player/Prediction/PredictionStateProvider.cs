using Input;
using Player.Db;
using Player.Interface.Local;

namespace Player.Prediction
{
    public class PredictionStateProvider : IPredictionStateProvider
    {
        private long _inputTick;
        
        public PredictionStateFrame Read(InputFrame inputFrame, ClientStateFrame clientStateFrame)
        {
            if (_inputTick == long.MaxValue)
                _inputTick = 0;
            
            _inputTick++;
            
            return new PredictionStateFrame()
            {
                InputTick = _inputTick,
                Movement = inputFrame.Movement,
                Look = inputFrame.Look,
                Origin = inputFrame.Origin,
                Direction = inputFrame.Direction,
                Aim = inputFrame.Aim,
                Jump = inputFrame.Jump,
                Run = inputFrame.Run,
                AimDirection = clientStateFrame.AimDirection,
                AimPitch = clientStateFrame.AimPitch,
                RotationY = clientStateFrame.RotationY,
            };
        }
    }
}