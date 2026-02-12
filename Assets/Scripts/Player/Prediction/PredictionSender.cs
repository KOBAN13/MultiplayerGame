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

            var windowSize = GetResendWindowSize();

            if (_preconditionStorage.CopyLast(windowSize, _batchFrames) == 0)
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

        private int GetResendWindowSize()
        {
            var localRate = _predictionParameters.CountGenerateStateLocalSimulation;
            var sendRate = _predictionParameters.CountGenerateStateSendToServer;

            if (localRate <= 0 || sendRate <= 0)
                return 1;

            var ratio = (localRate + sendRate - 1) / sendRate;
            var window = ratio * 2;
            var maxBuffer = _predictionParameters.MaxBufferSize;

            if (maxBuffer > 0 && window > maxBuffer)
                window = maxBuffer;

            return Math.Max(1, window);
        }

        private void SendBatch(List<PredictionStateFrame> frames)
        {
            var data = SFSObject.NewInstance();
            var inputs = new SFSArray();

            for (var i = 0; i < frames.Count; i++)
            {
                var frame = frames[i];
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
                input.PutLong("inputTick", frame.InputTick);

                inputs.AddSFSObject(input);
            }

            data.PutInt("count", frames.Count);
            data.PutLong("startTick", frames[0].InputTick);
            data.PutLong("endTick", frames[^1].InputTick);
            data.PutSFSArray("inputs", inputs);

            _sfs.Send(new ExtensionRequest(SFSResponseHelper.PLAYER_PRECONDITION_STATE_BATCH, data, _sfs.LastJoinedRoom));
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}
