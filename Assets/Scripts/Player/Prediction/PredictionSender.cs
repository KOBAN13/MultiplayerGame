using System;
using System.Collections.Generic;
using Db.Interface;
using Helpers;
using Player.Db;
using Player.Interface.Local;
using R3;
using Services.Interface;
using Sfs2X;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using VContainer.Unity;

namespace Player.Prediction
{
    public class PredictionSender : IInitializable, IDisposable
    {
        private readonly IPredictionParameters _predictionParameters;
        private readonly IPredictionBuffer _predictionBuffer;
        private readonly IPreconditionStorageService _preconditionStorage;
        private readonly SmartFox _sfs;
        
        private readonly List<PredictionStateFrame> _batchFrames = new();
        private readonly CompositeDisposable _disposable = new ();
        private long _networkInputTick;

        public PredictionSender(
            IPredictionParameters predictionParameters,
            IPredictionBuffer predictionBuffer,
            IPreconditionStorageService preconditionStorage, 
            SmartFox sfs
        )
        {
            _predictionParameters = predictionParameters;
            _predictionBuffer = predictionBuffer;
            _preconditionStorage = preconditionStorage;
            _sfs = sfs;
        }
        
        public void Initialize()
        {
            var sendRate = Math.Max(1, _predictionParameters.CountGenerateStateSendToServer);
            var period = TimeSpan.FromSeconds(1f / sendRate);

            Observable
                .Interval(period)
                .Subscribe(_ => TickSend())
                .AddTo(_disposable);
        }

        private void TickSend()
        {
            if (!DrainNewFrames())
                return;

            if (_preconditionStorage.CopyLast(1, _batchFrames) == 0)
                return;

            SendBatch(_batchFrames);
        }

        private bool DrainNewFrames()
        {
            var hasFrames = false;

            while (_predictionBuffer.TryDequeue(out var predictionStateFrame))
            {
                hasFrames = true;
                _preconditionStorage.AddPrecondition(in predictionStateFrame);
            }

            return hasFrames;
        }

        private void SendBatch(List<PredictionStateFrame> frames)
        {
            var data = SFSObject.NewInstance();
            var inputs = new SFSArray();

            foreach (var frame in frames)
            {
                var inputTick = GetNextInputTick();
                var input = SFSObject.NewInstance();
                input.PutFloat("horizontal", frame.Movement.x);
                input.PutFloat("vertical", frame.Movement.z);
                input.PutBool("isJumping", frame.Jump);
                input.PutBool("isRunning", frame.Run);
                input.PutFloat("eulerAngleY", frame.RotationY);
                input.PutFloat("aimDirectionX", frame.AimDirection.x);
                input.PutFloat("aimDirectionY", frame.AimDirection.y);
                input.PutFloat("aimDirectionZ", frame.AimDirection.z);
                input.PutFloat("aimPitch", frame.AimPitch);
                input.PutLong("inputTick", inputTick);

                inputs.AddSFSObject(input);
            }

            var startTick = _networkInputTick - frames.Count + 1;
            data.PutLong("startTick", startTick);
            data.PutLong("endTick", _networkInputTick);
            data.PutSFSArray("inputs", inputs);

            _sfs.Send(new ExtensionRequest(SFSResponseHelper.PLAYER_PRECONDITION_STATE, data, _sfs.LastJoinedRoom));
        }

        private long GetNextInputTick()
        {
            if (_networkInputTick == long.MaxValue)
                _networkInputTick = 0;

            _networkInputTick++;
            return _networkInputTick;
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}
