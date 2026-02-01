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
        private readonly ParrelSyncRuntime _parrelSyncRuntime;
        private readonly CompositeDisposable  _disposables = new();
        private readonly bool _forceLeftMovement;
        private int _sequenceId;
        
        private float _oscillationStartTime;
        private const float OscillationIntervalSeconds = 1f;
        
        public ReactiveCommand<bool> AimCommand { get; private set; } = new();
        public ReactiveCommand<bool> ShotCommand { get; private set; } = new();

        public LocalInputSource(IPlayerNetworkInputReader playerNetworkInputReader, ParrelSyncRuntime parrelSyncRuntime)
        {
            _playerNetworkInputReader = playerNetworkInputReader;
            _parrelSyncRuntime = parrelSyncRuntime;
            _forceLeftMovement = _parrelSyncRuntime.IsAutoLeftClone();
        }
        
        public InputFrame Read()
        {
            var movement = _playerNetworkInputReader.Movement.CurrentValue;
            
            if (_forceLeftMovement)
            {
                var elapsed = Time.time - _oscillationStartTime;
                var goLeft = Mathf.FloorToInt(elapsed / OscillationIntervalSeconds) % 2 == 0;
                movement = goLeft ? new Vector3(-1f, 0f, 0f) : new Vector3(1f, 0f, 0f);
            }

            return new InputFrame
            {
                Movement = movement,
                Look = _playerNetworkInputReader.Look.CurrentValue,
                Jump = _playerNetworkInputReader.Jump.CurrentValue,
                Run = _playerNetworkInputReader.Run.CurrentValue,
                Aim = _playerNetworkInputReader.Aim.CurrentValue,
                Shoot = _playerNetworkInputReader.Shoot.CurrentValue,
                Origin = _playerNetworkInputReader.Origin,
                Direction = _playerNetworkInputReader.Direction,
                Time = Time.time,
                SequenceId = _sequenceId++
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
