using System;
using Db.Interface;
using Input;
using Player.Interface.Local;
using Player.Weapon;
using R3;
using Sfs2X;
using UnityEngine;
using Utils.Enums;
using VContainer;

namespace Player.Local
{
    public class LocalPlayerMotor : APlayer
    {
        [SerializeField] private Transform _cameraTarget;
        [SerializeField] private AWeapon _currentWeapon;
        
        private IInputSource _inputSource;
        private IPlayerNetworkInputSender _playerNetworkInputSender;
        private IRotationCameraParameters _rotationCameraParameters;
        private IPlayerCameraHolder _playerCameraHolder;
        private SmartFox _sfs;
        
        private InputFrame _lastInputFrame;
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;
        private UnityEngine.Camera _mainCamera;
        private const float THRESHOLD = 0.01f;
        
        private Func<SmartFox, CharacterController, Transform, IPlayerNetworkInputSender> _playerNetworkInputSenderFactory;

        [Inject]
        public void Construct(
            IInputSource inputSource,
            SmartFox sfs,
            IRotationCameraParameters rotationCameraParameters,
            Func<SmartFox, CharacterController, Transform, IPlayerNetworkInputSender> playerNetworkInputSenderFactory,
            IPlayerCameraHolder playerCameraHolder
        )
        {
            _inputSource = inputSource;
            _sfs = sfs;
            _rotationCameraParameters = rotationCameraParameters;
            _playerNetworkInputSenderFactory = playerNetworkInputSenderFactory;
            _playerCameraHolder = playerCameraHolder;

            _playerNetworkInputSender = _playerNetworkInputSenderFactory(
                _sfs,
                CharacterController,
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

        public void OnEnable()
        {
            _mainCamera = UnityEngine.Camera.main;
            
            _inputSource.AimCommand
                .Subscribe(isAim =>
                {
                    _playerCameraHolder.SetVirtualCamera(isAim
                        ? EVirtualCameraType.Aim
                        : EVirtualCameraType.Gameplay);
                })
                .AddTo(this);
            
            _inputSource.ShotCommand
                .Where(isShot => isShot && _lastInputFrame.Aim)
                .Subscribe(_ => _currentWeapon.Attack())
                .AddTo(this);
        }

        public void Update()
        {
            _lastInputFrame = _inputSource.Read();
            _playerNetworkInputSender.SendServerPlayerInput(_lastInputFrame);
            SnapshotMotor.Tick(_lastInputFrame.Aim); 
            
            if (_lastInputFrame.Aim)
            {
                LocalRotate();
            }
        }

        public void LateUpdate()
        {
            _cameraTarget.rotation = RotateCamera(_lastInputFrame.Look);
        }

        private void LocalRotate()
        {
            var ray = _mainCamera.ScreenPointToRay(_lastInputFrame.Look);
            var plane = new Plane(Vector3.up, transform.position);

            if (!plane.Raycast(ray, out var enter))
            {
                return;
            }

            var worldAimTarget = ray.GetPoint(enter);
            worldAimTarget.y = transform.position.y;

            var aimDirection = worldAimTarget - transform.position;
            
            var targetRotation = Quaternion.LookRotation(aimDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 20f);
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
