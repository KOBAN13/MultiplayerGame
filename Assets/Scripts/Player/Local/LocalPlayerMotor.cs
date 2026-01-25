using System;
using Db.Interface;
using Input;
using Player.Db;
using Player.Interface.Local;
using Player.Weapon;
using R3;
using UnityEngine;
using Utils.Enums;
using VContainer;

namespace Player.Local
{
    public class LocalPlayerMotor : APlayer
    {
        [SerializeField] private Transform _cameraTarget;
        [SerializeField] private Transform _yawTarget;
        [SerializeField] private AWeapon _currentWeapon;
        
        private IInputSource _inputSource;
        private IClientStateProvider _clientStateProvider;
        private IPlayerNetworkStateSender _playerNetworkStateSender;
        private IRotationCameraParameters _rotationCameraParameters;
        private IPlayerCameraHolder _playerCameraHolder;
        
        private readonly RaycastHit[] _hits = new RaycastHit[1];
        private InputFrame _lastInputFrame;
        private ClientStateFrame _lastClientStateFrame;
        private Vector3 _aimDirection;
        private float _targetYaw;
        private float _targetPitch;
        private UnityEngine.Camera _mainCamera;

        private const float THRESHOLD = 0.01f;

        [Inject]
        public void Construct(
            IInputSource inputSource,
            IRotationCameraParameters rotationCameraParameters,
            Func<CharacterController, ClientStateProvider> clientStateProviderFactory,
            IPlayerCameraHolder playerCameraHolder,
            IPlayerNetworkStateSender playerNetworkStateSender
        )
        {
            _playerNetworkStateSender = playerNetworkStateSender;
            _inputSource = inputSource;
            _rotationCameraParameters = rotationCameraParameters;
            _playerCameraHolder = playerCameraHolder;
            
            _clientStateProvider = clientStateProviderFactory(CharacterController);
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
                .Subscribe(OnPlayerAim)
                .AddTo(this);
            
            _inputSource.ShotCommand
                .Where(isShot => isShot && _lastInputFrame.Aim)
                .Subscribe(_ => OnPlayerAttack())
                .AddTo(this);
        }

        private void OnPlayerAttack()
        {
            var fireCommand = new FireCommand
            {
                snapshotId = SnapshotsService.GetSnapshotId(),
                shotData = new ShotData
                {
                    origin = _lastInputFrame.Origin,
                    direction = _lastInputFrame.Direction,
                }
            };

            _currentWeapon.Attack(ref fireCommand);
        }

        public void Update()
        {
            _clientStateProvider.Write(_mainCamera.transform.rotation.eulerAngles.y, _aimDirection, _targetPitch);
            _lastInputFrame = _inputSource.Read();
            _lastClientStateFrame = _clientStateProvider.Read(_lastInputFrame);
            _playerNetworkStateSender.SendServerPlayerState(_lastInputFrame);
            _playerNetworkStateSender.SendServerPlayerInput(_lastClientStateFrame);
            SnapshotMotor.Tick(_lastInputFrame.Aim); 
        }

        public void LateUpdate()
        {
            _cameraTarget.rotation = RotateCamera(_lastInputFrame.Look);
            
            if (_lastInputFrame.Aim)
            {
                LocalRotate();
            }
        }
        
        private void OnPlayerAim(bool isAim)
        {
            _playerCameraHolder.SetVirtualCamera(isAim
                ? EVirtualCameraType.Aim
                : EVirtualCameraType.Gameplay);

            if (!isAim)
                _yawTarget.transform.localRotation = Quaternion.identity;
        }

        private void LocalRotate()
        {
            var screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
            
            var ray = _mainCamera.ScreenPointToRay(screenCenterPoint);
            
            var count = Physics.RaycastNonAlloc(
                ray,
                _hits, 
                _rotationCameraParameters.RaycastDistance, 
                _rotationCameraParameters.AimColliderLayerMask,
                QueryTriggerInteraction.Ignore
            );
            
            var mouseWorldPosition = count > 0 ? 
                _hits[0].point 
                : ray.GetPoint(_rotationCameraParameters.RaycastDistance);
            
            var direction = mouseWorldPosition - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude < 0.0001f)
                return;

            var targetRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _rotationCameraParameters.RotateSpeed);
            
            var cameraAngleOverride = _rotationCameraParameters.AngleOverride;
            
            var yawRotation = Quaternion.Euler(_targetPitch + cameraAngleOverride, 0.0f, 0.0f);
            
            _yawTarget.localRotation = Quaternion.Slerp(
                _yawTarget.localRotation,
                yawRotation,
                Time.deltaTime * _rotationCameraParameters.RotateSpeed);
            
            _aimDirection = direction.normalized;
        }
        
        private Quaternion RotateCamera(Vector2 position)
        {
            var bottomClamp = _rotationCameraParameters.BottomClamp;
            var topClamp = _rotationCameraParameters.TopClamp;
            var cameraAngleOverride = _rotationCameraParameters.AngleOverride;
            var sensitivity = _rotationCameraParameters.Sensitivity;
            
            if (position.sqrMagnitude >= THRESHOLD)
            {
                _targetYaw += position.x * Time.deltaTime * sensitivity;
                _targetPitch += position.y * Time.deltaTime * sensitivity;
            }
            
            _targetYaw = ClampAngle(_targetYaw, float.MinValue, float.MaxValue);
            _targetPitch = ClampAngle(_targetPitch, bottomClamp, topClamp);
            
            return Quaternion.Euler(_targetPitch + cameraAngleOverride, _targetYaw, 0.0f);
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
