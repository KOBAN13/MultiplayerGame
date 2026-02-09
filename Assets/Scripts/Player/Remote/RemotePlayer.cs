using System;
using Db.Interface;
using Player.Interface;
using Services.Interface;
using UnityEngine;
using VContainer;

namespace Player.Remote
{
    public class RemotePlayer : APlayer
    {
        [SerializeField] private Transform _visualRoot;

        private Vector3 _visualVelocity;
        private Vector3 _visualWorldPos;
        private Quaternion _visualWorldRot;
        private Vector3 _visualLocalOffset;
        private Quaternion _visualRotationOffset;
        
        private IPlayerSnapshotMotor _snapshotMotor;
        private IRemotePlayerParameters _remotePlayerParameters;

        [Inject]
        private void Construct(IRemotePlayerParameters remotePlayerParameters, 
            Func<ISnapshotsService, Transform, IRemotePlayerParameters, IPlayerSnapshotMotor> snapshotMotorFactory
        )
        {
            _remotePlayerParameters = remotePlayerParameters;
            
            _snapshotMotor = snapshotMotorFactory(
                SnapshotsService,
                RootTransform, 
                _remotePlayerParameters);
        }

        private void Awake()
        {
            _visualLocalOffset = RootTransform.InverseTransformPoint(_visualRoot.position);
            _visualRotationOffset = Quaternion.Inverse(RootTransform.rotation) * _visualRoot.rotation;
            _visualWorldPos = _visualRoot.position;
            _visualWorldRot = _visualRoot.rotation;
        }

        private void Update()
        {
            _snapshotMotor.Tick(false);
        }

        private void LateUpdate()
        {
            var targetPosition = RootTransform.TransformPoint(_visualLocalOffset);
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
                RootTransform.rotation * _visualRotationOffset,
                Time.deltaTime * visualRotationLerpSpeed);

            _visualRoot.position = _visualWorldPos;
            _visualRoot.rotation = _visualWorldRot;
        }
    }
}
