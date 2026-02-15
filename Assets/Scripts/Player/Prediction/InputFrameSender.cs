using System;
using System.Collections.Generic;
using Db.Interface;
using Helpers;
using Input;
using Player.Interface.Local;
using R3;
using Services.Interface;
using Sfs2X;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using VContainer.Unity;

namespace Player.Prediction
{
    public class InputFrameSender : IInitializable, IDisposable
    {
        private readonly IPredictionParameters _predictionParameters;
        private readonly IInputFrameBuffer _inputFrameBuffer;
        private readonly IPreconditionStorageService _preconditionStorage;
        private readonly SmartFox _sfs;
        
        private readonly List<InputFrame> _batchFrames = new();
        private readonly CompositeDisposable _disposable = new ();

        public InputFrameSender(
            IPredictionParameters predictionParameters,
            IInputFrameBuffer inputFrameBuffer,
            IPreconditionStorageService preconditionStorage, 
            SmartFox sfs
        )
        {
            _predictionParameters = predictionParameters;
            _inputFrameBuffer = inputFrameBuffer;
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
            var newFramesCount = DrainNewFrames();
            
            if (newFramesCount == 0)
                return;

            if (_preconditionStorage.CopyLast(newFramesCount, _batchFrames) == 0)
                return;

            SendBatch(_batchFrames);
        }

        private int DrainNewFrames()
        {
            var count = 0;

            while (_inputFrameBuffer.TryDequeue(out var predictionStateFrame))
            {
                count++;
            }

            return count;
        }

        private void SendBatch(List<InputFrame> frames)
        {
            if (frames.Count == 0 || _sfs.LastJoinedRoom == null)
                return;
            
            var data = SFSObject.NewInstance();
            var inputs = new SFSArray();

            foreach (var frame in frames)
            {
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

            data.PutLong("startTick", frames[0].InputTick);
            data.PutLong("endTick", frames[^1].InputTick);
            data.PutSFSArray("inputs", inputs);

            _sfs.Send(new ExtensionRequest(SFSResponseHelper.PLAYER_PRECONDITION_STATE, data, _sfs.LastJoinedRoom));
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}
