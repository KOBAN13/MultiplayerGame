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
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private float _jumpVelocity = 5f;
        [SerializeField] private float _gravity = -9.81f;
        [SerializeField] private float _maxJumpHeight = 2f;
        [SerializeField] private float _stepThresholdSqr = 1f;
        
        private IInputSource _inputSource;
        private IClientStateProvider _clientStateProvider;
        private IPlayerNetworkStateSender _playerNetworkStateSender;
        private IRotationCameraParameters _rotationCameraParameters;
        private IPlayerCameraHolder _playerCameraHolder;
        private IPredictionStateProvider _predictionStateProvider;
        private IPredictionBuffer _predictionBuffer;
        private IPredictionParameters _predictionParameters;
        
        private readonly RaycastHit[] _hits = new RaycastHit[1];
        private InputFrame _lastInputFrame;
        private ClientStateFrame _lastClientStateFrame;
        private PredictionStateFrame _lastPredictionStateFrame;
        private Vector3 _aimDirection;
        private float _targetYaw;
        private float _targetPitch;
        private UnityEngine.Camera _mainCamera;
        private Vector3 _targetDirection = Vector3.forward;
        private float _verticalVelocity;
        private bool _isOnGround = true;

        private const float THRESHOLD = 0.01f;

        [Inject]
        public void Construct(
            IInputSource inputSource,
            IRotationCameraParameters rotationCameraParameters,
            IPlayerCameraHolder playerCameraHolder,
            IPlayerNetworkStateSender playerNetworkStateSender,
            IClientStateProvider clientStateProvider,
            IPredictionStateProvider predictionStateProvider,
            IPredictionBuffer predictionBuffer,
            IPredictionParameters predictionParameters
        )
        {
            _playerNetworkStateSender = playerNetworkStateSender;
            _inputSource = inputSource;
            _rotationCameraParameters = rotationCameraParameters;
            _playerCameraHolder = playerCameraHolder;
            _clientStateProvider = clientStateProvider;
            _predictionStateProvider = predictionStateProvider;
            _predictionBuffer = predictionBuffer;
            _predictionParameters = predictionParameters;
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
            
            var localSimulationRate = Math.Max(1, _predictionParameters.CountGenerateStateLocalSimulation);
            var period = TimeSpan.FromSeconds(1f / localSimulationRate);
            var time = 1f / localSimulationRate;
            
            Observable.Interval(period)
                .Subscribe(_ => CharacterMove(time))
                .AddTo(this);
        }

        private void OnPlayerAttack()
        {
            _lastInputFrame = _inputSource.Read();

            var render = SnapshotsService.GetRenderSnapshotId();
            
            var fireCommand = new FireCommand
            {
                snapshotId = render.snapshotId,
                alpha = render.alpha,
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
            //_playerNetworkStateSender.SendServerPlayerState(_lastInputFrame);
            //_playerNetworkStateSender.SendServerPlayerInput(_lastClientStateFrame);
        }
        
        public void LateUpdate()
        {
            _cameraTarget.rotation = RotateCamera(_lastInputFrame.Look);
            
            if (_lastInputFrame.Aim)
            {
                LocalRotate();
            }
        }

        private void CharacterMove(float time)
        {
            _lastPredictionStateFrame = _predictionStateProvider.Read(_lastInputFrame, _lastClientStateFrame);
            _predictionBuffer.Enqueue(_lastPredictionStateFrame);
            
            var targetSpeed = _lastPredictionStateFrame.Run ? 8f : 4f;

            var inputDirection = new Vector3(_lastPredictionStateFrame.Movement.x, 0f, _lastPredictionStateFrame.Movement.z);
            
            if (inputDirection.sqrMagnitude <= 0f)
                targetSpeed = 0f;

            var basePosition = transform.position;
            var baseX = basePosition.x;
            var baseY = basePosition.y;
            var baseZ = basePosition.z;

            if (inputDirection.sqrMagnitude > 0f)
            {
                var rotation = Mathf.Atan2(_lastPredictionStateFrame.Movement.x, _lastPredictionStateFrame.Movement.z) * Mathf.Rad2Deg
                               + _lastPredictionStateFrame.RotationY;
                
                var targetRotation = Quaternion.Euler(0f, rotation, 0f);
                _targetDirection = targetRotation * Vector3.forward;
                transform.rotation = targetRotation;
            }

            var dx = _targetDirection.x * targetSpeed * time;
            var dz = _targetDirection.z * targetSpeed * time;
            var stepSqr = dx * dx + dz * dz;

            if (stepSqr > _stepThresholdSqr)
                return;

            var targetX = baseX + dx;
            var targetZ = baseZ + dz;

            if (_lastPredictionStateFrame.Jump)
            {
                if (_isOnGround)
                {
                    _verticalVelocity = _jumpVelocity;
                    _isOnGround = false;
                }
            }

            _verticalVelocity += _gravity * time;
            var targetY = baseY + _verticalVelocity * time;

            if (targetY > _maxJumpHeight)
            {
                targetY = _maxJumpHeight;
                _verticalVelocity = 0f;
            }

            if (targetY <= 0f)
            {
                targetY = 0f;
                _verticalVelocity = 0f;
                _isOnGround = true;
            }
            else
            {
                _isOnGround = false;
            }

            var targetPosition = new Vector3(targetX, targetY, targetZ);
            var move = targetPosition - basePosition;

            if (_characterController != null)
                _characterController.Move(move);
            else
                transform.position = targetPosition;
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
