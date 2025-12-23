using UnityEngine;

namespace Input
{
    public class LocalInputSource : IInputSource
    {
        private readonly IPlayerNetworkInputReader _playerNetworkInputReader;
        private int _sequenceId;

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
    }
}
