using System;
using Db.Interface;
using Player.Interface;
using Player.Shared;
using UnityEngine;
using VContainer;

namespace Player.Remote
{
    public class RemotePlayer : APlayer
    {
        [SerializeField] private Transform _visualRoot;
        [SerializeField] private Transform _physicalRoot;

        private Vector3 _visualVelocity;
        private Vector3 _visualWorldPos;
        private Quaternion _visualWorldRot;
        private Vector3 _visualLocalOffset;
        private Quaternion _visualRotationOffset;
        
        private IPlayerSnapshotMotor _snapshotMotor;
        private IRemotePlayerParameters _remotePlayerParameters;
        private Func<SnapshotCharacterMotorArgs, IPlayerSnapshotMotor> _snapshotMotorFactory;

        [Inject]
        private void Construct(
            IRemotePlayerParameters remotePlayerParameters,
            Func<SnapshotCharacterMotorArgs, IPlayerSnapshotMotor> snapshotMotorFactory
        )
        {
            _remotePlayerParameters = remotePlayerParameters;
            _snapshotMotorFactory = snapshotMotorFactory;
        }

        protected override void OnSnapshotServiceInitialized()
        {
            _snapshotMotor = _snapshotMotorFactory(
                new SnapshotCharacterMotorArgs(
                    SnapshotsService,
                    _physicalRoot,
                    _visualRoot,
                    YawTarget,
                    _remotePlayerParameters));
        }

        private void Awake()
        {
            _visualLocalOffset = _physicalRoot.InverseTransformPoint(_visualRoot.position);
            _visualRotationOffset = Quaternion.Inverse(_physicalRoot.rotation) * _visualRoot.rotation;
            _visualWorldPos = _visualRoot.position;
            _visualWorldRot = _visualRoot.rotation;
        }

        private void Update()
        {
            _snapshotMotor.Tick();
        }

        private void LateUpdate()
        {
            var targetPosition = _physicalRoot.TransformPoint(_visualLocalOffset);
            var toTarget = targetPosition - _visualWorldPos;

            var visualSnapDistance = _remotePlayerParameters.VisualSnapDistance;
            var visualPositionSmoothTime = _remotePlayerParameters.VisualPositionSmoothTime;
            var visualRotationLerpSpeed = _remotePlayerParameters.VisualRotationLerpSpeed;
            
            if (toTarget.sqrMagnitude > visualSnapDistance * visualSnapDistance)
            {
                _visualWorldPos = targetPosition;
                _visualVelocity = Vector3.zero;
            }
            else
            {
                _visualWorldPos = Vector3.SmoothDamp(
                    _visualWorldPos,
                    targetPosition,
                    ref _visualVelocity,
                    visualPositionSmoothTime);
            }

            _visualWorldRot = Quaternion.Slerp(
                _visualWorldRot,
                _physicalRoot.rotation * _visualRotationOffset,
                Time.deltaTime * visualRotationLerpSpeed);

            _visualRoot.position = _visualWorldPos;
            _visualRoot.rotation = _visualWorldRot;
        }
    }
}
