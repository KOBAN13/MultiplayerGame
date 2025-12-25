using System;
using Db.Interface;
using Input;
using Player.Interface.Local;
using Sfs2X;
using UnityEngine;
using VContainer;

namespace Player.Local
{
    public class LocalPlayerController : MonoBehaviour, ILocalPlayerMotor
    {
        [SerializeField] private Transform _cameraTarget;
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private Animator _animator;
        
        private IInputSource _inputSource;
        private IPlayerNetworkInputSender _playerNetworkInputSender;
        private ILocalPlayerMotor _localPlayerMotor;
        private IRotationCameraParameters _rotationCameraParameters;
        private SmartFox _sfs;
        
        private InputFrame _lastInputFrame;
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;
        private float _rotationVelocity;
        private const float THRESHOLD = 0.01f;
        
        private Func<SmartFox, CharacterController, Transform, IPlayerNetworkInputSender> _playerNetworkInputSenderFactory;

        [Inject]
        public void Construct(IInputSource inputSource, IPlayerNetworkInputSender playerNetworkInputSender, SmartFox sfs)
        {
            _inputSource = inputSource;
            _playerNetworkInputSender = playerNetworkInputSender;
            _sfs = sfs;


            _playerNetworkInputSender = _playerNetworkInputSenderFactory(
                _sfs,
                _characterController,
                _cameraTarget);
        }
        
        public Transform GetTransform()
        {
            return transform;
        }

        public Transform GetCameraTarget()
        {
            return _cameraTarget;
        }

        public void Update()
        {
            _lastInputFrame = _inputSource.Read();
            _playerNetworkInputSender.SendServerPlayerInput(_lastInputFrame);
        }

        public void LateUpdate()
        {
            _cameraTarget.rotation = RotateCamera(_lastInputFrame.Look);
        }
        
        private Quaternion RotateCamera(Vector2 position)
        {
            var bottomClamp = _rotationCameraParameters.BottomClamp;
            var topClamp = _rotationCameraParameters.TopClamp;
            var cameraAngleOverride = _rotationCameraParameters.AngleOverride;
            var sensitivity = _rotationCameraParameters.Sensitivity;
            
            if (position.sqrMagnitude >= THRESHOLD)
            {
                _cinemachineTargetYaw += position.x * Time.deltaTime * sensitivity;
                _cinemachineTargetPitch += position.y * Time.deltaTime * sensitivity;
            }
            
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, bottomClamp, topClamp);
            
            return Quaternion.Euler(_cinemachineTargetPitch + cameraAngleOverride, _cinemachineTargetYaw, 0.0f);
        }
        
        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) 
                lfAngle += 360f;
            
            if (lfAngle > 360f) 
                lfAngle -= 360f;
            
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }
    }
}