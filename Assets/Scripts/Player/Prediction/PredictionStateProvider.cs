using Player.Db;
using Player.Interface.Local;
using UnityEngine;

namespace Player.Prediction
{
    public class PredictionStateProvider : IPredictionStateProvider
    {
        private Vector3 _position;
        private Vector3 _velocity;
        private Vector3 _moveDirection;
        private float _rotation;
        private bool _isGrounded;
        private long _inputTick;

        public void Write(
            Vector3 position,
            Vector3 velocity,
            Vector3 moveDirection,
            bool isGrounded,
            float rotation,
            int animationState,
            long inputTick)
        {
            _position = position;
            _velocity = velocity;
            _moveDirection = moveDirection;
            _rotation = rotation;
            _isGrounded = isGrounded;
            _inputTick = inputTick;
        }
        
        public PredictionStateFrame Read()
        {
            return new PredictionStateFrame()
            {
                InputTick = _inputTick,
                Position = _position,
                Velocity = _velocity,
                MoveDirection = _moveDirection,
                IsGrounded = _isGrounded,
                Rotation = _rotation
            };
        }
    }
}
