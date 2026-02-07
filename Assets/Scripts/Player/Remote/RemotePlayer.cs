using UnityEngine;

namespace Player.Remote
{
    public class RemotePlayer : APlayer
    {
        [SerializeField] private Transform _visualRoot;
        [SerializeField] private bool _enableVisualSmoothing = true;
        [SerializeField] private float _visualPositionSmoothTime = 0.06f;
        [SerializeField] private float _visualRotationLerpSpeed = 12f;
        [SerializeField] private float _visualSnapDistance = 1.5f;

        private Vector3 _visualVelocity;
        private Vector3 _visualWorldPos;
        private Quaternion _visualWorldRot;
        private Vector3 _visualLocalOffset;
        private Quaternion _visualRotationOffset;
        private bool _visualInitialized;

        private void Awake()
        {
            if (RootTransform == null)
                RootTransform = transform;

            if (_visualRoot == null && Animator != null && Animator.transform != RootTransform)
                _visualRoot = Animator.transform;

            if (_visualRoot == null || _visualRoot == RootTransform)
                return;

            _visualLocalOffset = RootTransform.InverseTransformPoint(_visualRoot.position);
            _visualRotationOffset = Quaternion.Inverse(RootTransform.rotation) * _visualRoot.rotation;
            _visualWorldPos = _visualRoot.position;
            _visualWorldRot = _visualRoot.rotation;
            _visualInitialized = true;
        }

        private void Update()
        {
            SnapshotMotor.Tick(false);
        }

        private void LateUpdate()
        {
            if (!_enableVisualSmoothing || !_visualInitialized)
                return;

            var targetPosition = RootTransform.TransformPoint(_visualLocalOffset);
            var toTarget = targetPosition - _visualWorldPos;

            if (toTarget.sqrMagnitude > _visualSnapDistance * _visualSnapDistance)
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
                    _visualPositionSmoothTime);
            }

            _visualWorldRot = Quaternion.Slerp(
                _visualWorldRot,
                RootTransform.rotation * _visualRotationOffset,
                Time.deltaTime * _visualRotationLerpSpeed);

            _visualRoot.position = _visualWorldPos;
            _visualRoot.rotation = _visualWorldRot;
        }
    }
}
