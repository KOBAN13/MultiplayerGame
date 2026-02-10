using System;
using Db.Interface;
using Player.Db;
using Player.Interface.Local;
using R3;
using Services.Interface;
using VContainer.Unity;

namespace Player.Prediction
{
    public class PredictionSender : IInitializable, IDisposable
    {
        private readonly IPredictionParameters _predictionParameters;
        private readonly IPredictionBuffer _predictionBuffer;
        private readonly IPreconditionStorageService _preconditionStorage;
        
        private readonly CompositeDisposable _disposable = new ();

        public PredictionSender(
            IPredictionParameters predictionParameters,
            IPredictionBuffer predictionBuffer,
            IPreconditionStorageService preconditionStorage
        )
        {
            _predictionParameters = predictionParameters;
            _predictionBuffer = predictionBuffer;
            _preconditionStorage = preconditionStorage;
        }
        
        public void Initialize()
        {
            var period = TimeSpan.FromSeconds(1f / _predictionParameters.CountGenerateStateInSeconds);

            Observable
                .Interval(period)
                .Subscribe(_ =>
                {
                    if (!_predictionBuffer.TryDequeue(out var predictionStateFrame))
                        return;

                    Send(in predictionStateFrame);
                    Store(in predictionStateFrame);
                })
                .AddTo(_disposable);
        }
        
        private void Store(in PredictionStateFrame predictionStateFrame)
        {
            _preconditionStorage.AddPrecondition(in predictionStateFrame);
        }

        private void Send(in PredictionStateFrame predictionStateFrame)
        {
            
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}
