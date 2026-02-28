using System;
using Db.Interface;
using Input;
using Player.Animation;
using Player.Db;
using Player.Interface.Local;
using Player.Weapon;
using Player.Weapon.Data;
using R3;
using UnityEngine;
using Utils.Enums;
using VContainer;

namespace Player.Local
{
    public class LocalPlayerMotor : APlayer
    {
        [SerializeField] private Transform _cameraTarget;
        [SerializeField] private Transform _visualRoot;
        [SerializeField] private CharacterController _characterController;
        
        private IInputSource _inputSource;
        private IRotationCameraParameters _rotationCameraParameters;
        private IPlayerCameraHolder _playerCameraHolder;
        private IInputFrameSyncService _inputFrameSyncService;
        private IPredictionParameters _predictionParameters;
        private ILocalPlayerParameters _localPlayerParameters;
        
        private readonly RaycastHit[] _hits = new RaycastHit[1];
        private InputFrame _lastInputFrame;
        private WeaponInputFrame _lastWeaponInputFrame;
        private Vector3 _aimDirection;
        private Vector3 _visualCorrector;
        private Vector3 _visualLocalOffset;
        private Vector3 _cameraLocalOffsetFromVisual;
        private Quaternion _visualRotationOffset;
        private Vector3 _visualWorldPos;
        private Quaternion _visualWorldRot;
        private float _targetYaw;
        private float _targetPitch;
        private UnityEngine.Camera _mainCamera;
        private Vector3 _targetDirection = Vector3.forward;
        private float _verticalVelocity;
        private float _simulationDeltaTime;
        private bool _isOnGround;
        private bool _lastServerGrounded;
        private bool _hasServerGrounded;

        private const float THRESHOLD = 0.01f;
        private Transform PhysicalRoot => _characterController.transform;

        private void Awake()
        {
            ConfigureAnimator();

            var physicalRoot = PhysicalRoot;

            _visualLocalOffset = physicalRoot.InverseTransformPoint(_visualRoot.position);
            _visualRotationOffset = Quaternion.Inverse(physicalRoot.rotation) * _visualRoot.rotation;
            _cameraLocalOffsetFromVisual = _visualRoot.InverseTransformPoint(_cameraTarget.position);
            _visualWorldPos = _visualRoot.position;
            _visualWorldRot = _visualRoot.rotation;
        }

        [Inject]
        public void Construct(
            IInputSource inputSource,
            IRotationCameraParameters rotationCameraParameters,
            IPlayerCameraHolder playerCameraHolder,
            IInputFrameSyncService inputFrameSyncService,
            IPredictionParameters predictionParameters,
            ILocalPlayerParameters localPlayerParameters
        )
        {
            _inputSource = inputSource;
            _rotationCameraParameters = rotationCameraParameters;
            _playerCameraHolder = playerCameraHolder;
            _inputFrameSyncService = inputFrameSyncService;
            _predictionParameters = predictionParameters;
            _localPlayerParameters = localPlayerParameters;
        }
        
        public Transform GetTransform()
        {
            return PhysicalRoot;
        }

        public Transform GetCameraTarget()
        {
            return _cameraTarget;
        }

        public void AddVisualCorrection(Vector3 offset)
        {
            _visualCorrector += offset;
        }

        public void UpdateVisualSmoothing(float dt, float halfLife)
        {
            var factor = Mathf.Pow(0.5f, dt / Mathf.Max(0.0001f, halfLife));
            var lerp = 1f - factor;
            var physicalRoot = PhysicalRoot;
            
            _visualCorrector *= factor;

            var targetPosition = physicalRoot.TransformPoint(_visualLocalOffset) + _visualCorrector;
            var targetRotation = physicalRoot.rotation * _visualRotationOffset;

            _visualWorldPos = Vector3.Lerp(_visualWorldPos, targetPosition, lerp);
            _visualWorldRot = Quaternion.Slerp(_visualWorldRot, targetRotation, lerp);

            _visualRoot.position = _visualWorldPos;
            _visualRoot.rotation = _visualWorldRot;
            _cameraTarget.position = _visualRoot.TransformPoint(_cameraLocalOffsetFromVisual);
        }

        public bool TryHardMove(Vector3 delta)
        {
            var before = PhysicalRoot.position;
            _characterController.Move(delta);
            var after = PhysicalRoot.position;
            
            return (after - before).sqrMagnitude >= (delta.sqrMagnitude * 0.25f);
        }

        public void TeleportUnsafe(Vector3 position)
        {
            var wasEnabled = _characterController.enabled;

            if (wasEnabled)
                _characterController.enabled = false;

            PhysicalRoot.position = position;

            if (wasEnabled)
                _characterController.enabled = true;
            
            _visualCorrector = Vector3.zero;
            SnapVisualToPhysical();
        }

        public void OnEnable()
        {
            _mainCamera = UnityEngine.Camera.main;
            SnapVisualToPhysical();
            
            _inputSource.AimCommand
                .Subscribe(OnPlayerAim)
                .AddTo(this);
            
            _inputSource.ShotCommand
                .Where(isShot => isShot && _lastInputFrame.Aim)
                .Subscribe(_ => OnPlayerAttack())
                .AddTo(this);
            
            var localSimulationRate = Math.Max(1, _predictionParameters.CountGenerateStateLocalSimulation);
            var period = TimeSpan.FromSeconds(1f / localSimulationRate);
            _simulationDeltaTime = 1f / localSimulationRate;
            _inputFrameSyncService.Reset();
            
            Observable.Interval(period)
                .Subscribe(_ => CharacterMove())
                .AddTo(this);
        }

        private void SnapVisualToPhysical()
        {
            var physicalRoot = PhysicalRoot;
            _visualWorldPos = physicalRoot.TransformPoint(_visualLocalOffset) + _visualCorrector;
            _visualWorldRot = physicalRoot.rotation * _visualRotationOffset;

            _visualRoot.position = _visualWorldPos;
            _visualRoot.rotation = _visualWorldRot;
            _cameraTarget.position = _visualRoot.TransformPoint(_cameraLocalOffsetFromVisual);
        }

        private void OnPlayerAttack()
        {
            _lastWeaponInputFrame = _inputSource.ReadWeaponInput();

            var render = SnapshotsService.GetRenderSnapshotId();
            
            var fireCommand = new ServerFireCommand
            {
                snapshotId = render.snapshotId,
                alpha = render.alpha,
                shotData = new ShotData
                {
                    origin = _lastWeaponInputFrame.Origin,
                    direction = _lastWeaponInputFrame.Direction,
                }
            };

            CurrentWeapon.Attack(ref fireCommand);
        }

        public void Update()
        {
            _lastWeaponInputFrame = _inputSource.ReadWeaponInput();
        }
        
        public void LateUpdate()
        {
            _cameraTarget.rotation = RotateCamera(_lastWeaponInputFrame.Look);
            
            if (_lastInputFrame.Aim)
            {
                LocalRotate();
            }

            UpdateVisualSmoothing(Time.deltaTime, _predictionParameters.VisualHalfLife);
        }

        private void CharacterMove()
        {
            _lastInputFrame = _inputSource.ReadInputFrame(_mainCamera.transform.rotation.eulerAngles.y, _aimDirection, _targetPitch);

            var currentState = BuildPredictedState(0, 1);
            var predictedState = SimulatePredicted(in currentState, in _lastInputFrame, _simulationDeltaTime);

            UpdateAnimatorParameters(in _lastInputFrame, in predictedState);
            
            ApplyPredictedState(in predictedState, true);
            var groundedForSync = GetGroundedForSync();
            
            _inputFrameSyncService.CaptureAndQueue(
                in _lastInputFrame,
                _simulationDeltaTime,
                PhysicalRoot.position,
                new Vector3(0f, _verticalVelocity, 0f),
                _targetDirection,
                groundedForSync,
                _mainCamera.transform.rotation.eulerAngles.y,
                1);
        }

        private void UpdateAnimatorParameters(in InputFrame inputFrame, in PredictionStateFrame predictedState)
        {
            var inputDirection = new Vector3(inputFrame.Movement.x, 0f, inputFrame.Movement.z);
            var inputMagnitude = Mathf.Clamp01(inputDirection.magnitude);

            if (inputMagnitude <= 0.0001f)
            {
                UpdateMovementAnimation(Vector3.zero, PhysicalRoot.rotation, _simulationDeltaTime);
                return;
            }

            var locomotionScale = inputFrame.Run
                ? 1f
                : Mathf.Clamp01(_localPlayerParameters.SpeedWalk / Mathf.Max(_localPlayerParameters.SpeedRun, 0.001f));

            var scaledInputDirection = inputDirection.normalized * (inputMagnitude * locomotionScale);
            var worldDirection = Quaternion.Euler(0f, inputFrame.RotationY, 0f) * scaledInputDirection;
            var referenceRotation = inputFrame.Aim
                ? PhysicalRoot.rotation
                : Quaternion.Euler(0f, predictedState.Rotation, 0f);

            UpdateMovementAnimation(worldDirection, referenceRotation, _simulationDeltaTime);
        }

        public PredictionStateFrame SimulatePredicted(
            in PredictionStateFrame currentState,
            in InputFrame inputFrame,
            float deltaTime)
        {
            var nextState = currentState;
            nextState.InputTick = inputFrame.InputTick;

            var targetSpeed = inputFrame.Run ? _localPlayerParameters.SpeedRun : _localPlayerParameters.SpeedWalk;
            var inputDirection = new Vector3(inputFrame.Movement.x, 0f, inputFrame.Movement.z);

            if (inputDirection.sqrMagnitude <= 0f)
                targetSpeed = 0f;

            var targetDirection = currentState.MoveDirection.sqrMagnitude > 0f
                ? currentState.MoveDirection.normalized
                : Vector3.forward;

            if (inputDirection.sqrMagnitude > 0f)
            {
                var rotation = Mathf.Atan2(inputFrame.Movement.x, inputFrame.Movement.z) * Mathf.Rad2Deg
                               + inputFrame.RotationY;
                
                nextState.Rotation = rotation;

                var targetRotation = Quaternion.Euler(0f, rotation, 0f);
                targetDirection = targetRotation * Vector3.forward;
            }

            var dx = targetDirection.x * targetSpeed * deltaTime;
            var dz = targetDirection.z * targetSpeed * deltaTime;
            
            nextState.MoveDirection = targetDirection;

            var verticalVelocity = currentState.Velocity.y;
            var isOnGround = currentState.IsGrounded;

            if (inputFrame.Jump && isOnGround)
            {
                verticalVelocity = _localPlayerParameters.JumpVelocity;
                isOnGround = false;
            }

            var targetPosition = currentState.Position;
            targetPosition.x += dx;
            targetPosition.z += dz;

            verticalVelocity += _localPlayerParameters.Gravity * deltaTime;
            targetPosition.y += verticalVelocity * deltaTime;

            if (targetPosition.y <= 0f)
            {
                targetPosition.y = 0f;
                verticalVelocity = 0f;
                isOnGround = true;
            }
            else
            {
                isOnGround = false;
            }

            nextState.Position = targetPosition;
            nextState.Velocity = new Vector3(0f, verticalVelocity, 0f);
            nextState.IsGrounded = isOnGround;
            
            //nextState.AnimationState = "Idle";

            return nextState;
        }

        public void ApplyPredictedState(in PredictionStateFrame state, bool isApplyPosition)
        {
            _verticalVelocity = state.Velocity.y;
            _isOnGround = state.IsGrounded;
            
            _targetDirection = state.MoveDirection.sqrMagnitude > 0f
                ? state.MoveDirection.normalized
                : Vector3.forward;

            PhysicalRoot.rotation = Quaternion.Euler(0f, state.Rotation, 0f);
            
            if (!isApplyPosition)
                return;
            
            var move = state.Position - PhysicalRoot.position;
            _characterController.Move(move);
        }

        private PredictionStateFrame BuildPredictedState(long inputTick, int animationState)
        {
            return new PredictionStateFrame()
            {
                InputTick = inputTick,
                Position = PhysicalRoot.position,
                Velocity = new Vector3(0f, _verticalVelocity, 0f),
                MoveDirection = _targetDirection.sqrMagnitude > 0f ? _targetDirection.normalized : Vector3.forward,
                Rotation = PhysicalRoot.rotation.eulerAngles.y,
                IsGrounded = _isOnGround,
                AnimationState = animationState
            };
        }

        public override void SetSnapshot(in SnapshotData snapshot)
        {
            _lastServerGrounded = snapshot.IsGrounded;
            _hasServerGrounded = true;
            base.SetSnapshot(in snapshot);
        }

        private bool GetGroundedForSync()
        {
            return _hasServerGrounded ? _lastServerGrounded : _isOnGround;
        }
        
        private void OnPlayerAim(bool isAim)
        {
            _playerCameraHolder.SetVirtualCamera(isAim
                ? EVirtualCameraType.Aim
                : EVirtualCameraType.Gameplay);

            if (!isAim)
                YawTarget.transform.localRotation = Quaternion.identity;
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
            
            var direction = mouseWorldPosition - PhysicalRoot.position;
            direction.y = 0f;

            if (direction.sqrMagnitude < 0.0001f)
                return;

            var targetRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
            PhysicalRoot.rotation = Quaternion.Slerp(PhysicalRoot.rotation, targetRotation, Time.deltaTime * _rotationCameraParameters.RotateSpeed);
            
            var cameraAngleOverride = _rotationCameraParameters.AngleOverride;
            
            var yawRotation = Quaternion.Euler(_targetPitch + cameraAngleOverride, 0.0f, 0.0f);
            
            YawTarget.localRotation = Quaternion.Slerp(
                YawTarget.localRotation,
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
