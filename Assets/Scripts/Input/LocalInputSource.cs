using System;
using R3;
using UnityEngine;
using VContainer.Unity;

namespace Input
{
    public class LocalInputSource : IInputSource, IDisposable, IInitializable
    {
        private readonly IPlayerNetworkInputReader _playerNetworkInputReader;
        private readonly CompositeDisposable  _disposables = new();
        private int _sequenceId;
        
        public ReactiveCommand<bool> AimCommand { get; private set; } = new();
        public ReactiveCommand<bool> ShotCommand { get; private set; } = new();

        public LocalInputSource(IPlayerNetworkInputReader playerNetworkInputReader)
        {
            _playerNetworkInputReader = playerNetworkInputReader;
        }
        
        public InputFrame Read()
        {
            return new InputFrame
            {
                Movement = _playerNetworkInputReader.Movement.CurrentValue,
                Look = _playerNetworkInputReader.Look.CurrentValue,
                Jump = _playerNetworkInputReader.Jump.CurrentValue,
                Run = _playerNetworkInputReader.Run.CurrentValue,
                Aim = _playerNetworkInputReader.Aim.CurrentValue,
                Shoot = _playerNetworkInputReader.Shoot.CurrentValue,
                Time = Time.time,
                SequenceId = _sequenceId++
            };
        }
        
        public void Initialize()
        {
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
