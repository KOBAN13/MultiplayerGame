using System;
using Db.Interface;
using Input;
using Player.Interface.Local;
using Services.Interface;
using UnityEngine;

namespace Player.Prediction
{
    public class InputFrameSyncService : IInputFrameSyncService
    {
        private readonly IPredictionParameters _predictionParameters;
        private readonly IInputFrameBuffer _inputFrameBuffer;
        private readonly IPredictionStateProvider _predictionStateProvider;
        private readonly IPreconditionStorageService _preconditionStorage;

        private float _sendAccumulator;
        private float _sendInterval;
        private long _inputTick;

        public InputFrameSyncService(
            IPredictionParameters predictionParameters,
            IInputFrameBuffer inputFrameBuffer,
            IPredictionStateProvider predictionStateProvider,
            IPreconditionStorageService preconditionStorage
        )
        {
            _predictionParameters = predictionParameters;
            _inputFrameBuffer = inputFrameBuffer;
            _predictionStateProvider = predictionStateProvider;
            _preconditionStorage = preconditionStorage;
            Reset();
        }

        public void Reset()
        {
            var sendRate = Math.Max(1, _predictionParameters.CountGenerateStateSendToServer);
            _sendInterval = 1f / sendRate;
            _sendAccumulator = 0f;
            _inputTick = 0;
        }

        public void CaptureAndQueue(
            in InputFrame inputFrame,
            float deltaTime,
            Vector3 position,
            bool isGrounded,
            float rotationY,
            string animationState)
        {
            _sendAccumulator += deltaTime;

            if (_sendAccumulator < _sendInterval)
                return;

            _sendAccumulator -= _sendInterval;

            var networkInputFrame = inputFrame;
            networkInputFrame.InputTick = GetNextInputTick();
            _inputFrameBuffer.Enqueue(in networkInputFrame);

            _predictionStateProvider.Write(
                position,
                isGrounded,
                rotationY,
                animationState,
                networkInputFrame.InputTick
            );

            var precondition = _predictionStateProvider.Read();
            _preconditionStorage.AddPrecondition(in precondition);
        }

        private long GetNextInputTick()
        {
            if (_inputTick == long.MaxValue)
                _inputTick = 0;

            _inputTick++;
            return _inputTick;
        }
    }
}
