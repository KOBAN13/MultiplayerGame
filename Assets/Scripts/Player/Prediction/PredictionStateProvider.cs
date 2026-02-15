using Player.Db;
using Player.Interface.Local;
using UnityEngine;

namespace Player.Prediction
{
    public class PredictionStateProvider : IPredictionStateProvider
    {
        private Vector3 _position;
        private float _rotation;
        private bool _isGrounded;
        private string _animationState;
        private long _inputTick;

        public void Write(Vector3 position, bool isGrounded, float rotation, string animationState, long inputTick)
        {
            _position = position;
            _rotation = rotation;
            _isGrounded = isGrounded;
            _animationState = animationState;
            _inputTick = inputTick;
        }
        
        public PredictionStateFrame Read()
        {
            return new PredictionStateFrame()
            {
                InputTick = _inputTick,
                Position = _position,
                IsGrounded = _isGrounded,
                AnimationState = _animationState,
                Rotation = _rotation
            };
        }
    }
}