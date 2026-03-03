using System;
using R3;
using UnityEngine;
using Utils;
using VContainer.Unity;

namespace Input
{
    public class LocalInputSource : IInputSource, IDisposable, IInitializable
    {
        private readonly IPlayerNetworkInputReader _playerNetworkInputReader;
        private readonly CompositeDisposable  _disposables = new();
        private readonly bool _forceLeftMovement;
        
        private float _oscillationStartTime;
        private const float OscillationIntervalSeconds = 3f;
        
        public ReactiveCommand<bool> AimCommand { get; private set; } = new();
        public ReactiveCommand<bool> ShotCommand { get; private set; } = new();

        public LocalInputSource(IPlayerNetworkInputReader playerNetworkInputReader, ParrelSyncRuntime parrelSyncRuntime)
        {
            _playerNetworkInputReader = playerNetworkInputReader;
            _forceLeftMovement = parrelSyncRuntime.IsAutoLeftClone();
        }
        
        public InputFrame ReadInputFrame(float rotationCameraY, Vector3 aimDirection, float aimPitch)
        {
            var movement = _playerNetworkInputReader.MovementDirection;
            
            if (_forceLeftMovement)
            {
                var elapsed = Time.time - _oscillationStartTime;
                var goLeft = Mathf.FloorToInt(elapsed / OscillationIntervalSeconds) % 2 == 0;
                movement = goLeft ? new Vector3(-1f, 0f, 0f) : new Vector3(1f, 0f, 0f);
            }

            return new InputFrame
            {
                Movement = movement,
                AimDirection = aimDirection,
                AimPitch = aimPitch,
                RotationY = rotationCameraY,
                Jump = _playerNetworkInputReader.Jump.CurrentValue,
                Run = _playerNetworkInputReader.Run.CurrentValue,
                Aim = _playerNetworkInputReader.Aim.CurrentValue,
            };
        }

        public WeaponInputFrame ReadWeaponInput()
        {
            return new WeaponInputFrame()
            {
                Look = _playerNetworkInputReader.Look.CurrentValue,
                Origin = _playerNetworkInputReader.Origin,
                Direction = _playerNetworkInputReader.Direction,
            };
        }
        
        public void Initialize()
        {
            if (_forceLeftMovement)
            {
                _oscillationStartTime = Time.time;
            }

            _playerNetworkInputReader.Aim
                .Subscribe(isAim => AimCommand.Execute(isAim))
                .AddTo(_disposables);
            
            _playerNetworkInputReader.Shoot
                .Subscribe(isShot => ShotCommand.Execute(isShot))
                .AddTo(_disposables);
        }
        
        public void Dispose()
        {
            _disposables.Clear();
            _disposables.Dispose();
        }
    }
}
