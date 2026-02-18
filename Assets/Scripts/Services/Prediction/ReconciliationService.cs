using System;
using System.Collections.Generic;
using Db.Interface;
using Input;
using Player;
using Player.Db;
using Player.Local;
using Services.Interface;
using UnityEngine;

namespace Services.Prediction
{
    public class ReconciliationService : IReconciliationService
    {
        private const float LITE_POSITION_ERROR_THRESHOLD = 0.2f;

        private readonly IPreconditionStorageService _preconditionStorage;
        private readonly IPredictionParameters _predictionParameters;
        private readonly List<InputFrame> _replayInputs = new();

        public ReconciliationService(
            IPreconditionStorageService preconditionStorage,
            IPredictionParameters predictionParameters
        )
        {
            _preconditionStorage = preconditionStorage;
            _predictionParameters = predictionParameters;
        }

        public void Reconciliation(APlayer player, Vector3 position, in SnapshotData snapshotData)
        {
            if (player is not LocalPlayerMotor localPlayerMotor)
                return;

            if (!_preconditionStorage.TryFindPreconditionState(out var preconditionState, snapshotData.LastProcessedInputSequence))
            {
                Debug.LogWarning($"Could not find precondition state for tick={snapshotData.LastProcessedInputSequence}");
                _preconditionStorage.ClearOldCommands(snapshotData.LastProcessedInputSequence);
                return;
            }

            var posErrorAtN  = (position - preconditionState.Position).magnitude;

            if (posErrorAtN <= LITE_POSITION_ERROR_THRESHOLD)
            {
                _preconditionStorage.ClearOldCommands(snapshotData.LastProcessedInputSequence);
                return;
            }

            var correctedStateAtN = BuildStateFromServerAtTickN(in preconditionState, in snapshotData);
            var localSimulationRate = Math.Max(1, _predictionParameters.CountGenerateStateLocalSimulation);
            var dt = 1f / localSimulationRate;

            _preconditionStorage.CopyAfterTick(snapshotData.LastProcessedInputSequence, _replayInputs);

            var predictedState = correctedStateAtN;

            foreach (var inputFrame in _replayInputs)
            {
                predictedState = localPlayerMotor.SimulatePredicted(in predictedState, in inputFrame, dt);
                _preconditionStorage.AddPrecondition(in predictedState);
            }

            ApplyHybridCorrection(localPlayerMotor, predictedState.Position);
            
            localPlayerMotor.ApplyPredictedState(in predictedState, false);
            _preconditionStorage.ClearOldCommands(snapshotData.LastProcessedInputSequence);
        }

        private void ApplyHybridCorrection(LocalPlayerMotor motor, Vector3 predictedPosition)
        {
            var currentPos = motor.GetTransform().position;
            var delta = predictedPosition - currentPos;
            var error = delta.magnitude;

            if (error <= _predictionParameters.SmallError)
            {
                motor.AddVisualCorrection(delta);
                return;
            }

            if (error <= _predictionParameters.MediumError)
            {
                var moveDelta = delta * _predictionParameters.MediumMoveGain;

                var movedOk = motor.TryHardMove(moveDelta);
                var newPosition = motor.GetTransform().position;
                var residual = predictedPosition - newPosition;
                motor.AddVisualCorrection(residual);
                
                if (!movedOk && residual.magnitude > _predictionParameters.LargeError)
                {
                    movedOk = motor.TryHardMove(residual);
                    
                    if (!movedOk)
                        motor.TeleportUnsafe(predictedPosition);
                }
                
                return;
            }

            if (error <= _predictionParameters.LargeError)
            {
                var movedOk = motor.TryHardMove(delta);
                
                if (!movedOk)
                    motor.TeleportUnsafe(predictedPosition);
                
                return;
            }
            
            motor.TeleportUnsafe(predictedPosition);
        }

        private static PredictionStateFrame BuildStateFromServerAtTickN(
            in PredictionStateFrame predictedStateAtN,
            in SnapshotData snapshotData)
        {
            var correctedStateAtN = predictedStateAtN;
            correctedStateAtN.InputTick = snapshotData.LastProcessedInputSequence;
            correctedStateAtN.Position = snapshotData.Position;
            correctedStateAtN.Rotation = snapshotData.Rotation;
            correctedStateAtN.IsGrounded = snapshotData.IsGrounded;
            correctedStateAtN.AnimationState = snapshotData.AnimationState;
            correctedStateAtN.MoveDirection = Quaternion.Euler(0f, snapshotData.Rotation, 0f) * Vector3.forward;

            if (correctedStateAtN.IsGrounded)
                correctedStateAtN.Velocity = Vector3.zero;

            return correctedStateAtN;
        }
    }
}
