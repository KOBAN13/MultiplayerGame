using System;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer.Unity;

namespace Input
{
    public class PlayerNetworkInputReader : IPlayerNetworkInputReader, PlayerInput.IMovementActions, IInitializable, IDisposable
    {
        private readonly PlayerInput _playerInput;
        private readonly ReactiveProperty<Vector3> _movement = new();
        private readonly ReactiveProperty<bool> _jump = new();
        private readonly ReactiveProperty<bool> _run = new();
        
        public ReadOnlyReactiveProperty<Vector3> Movement => _movement;
        public ReadOnlyReactiveProperty<bool> Jump => _jump;
        public ReadOnlyReactiveProperty<bool> Run => _run;
        
        public PlayerNetworkInputReader(PlayerInput playerInput)
        {
            _playerInput = playerInput;
        }
        
        public void Initialize()
        {
            _playerInput.Movement.SetCallbacks(this);
            _playerInput.Enable();
        }
        
        public void OnMovement(InputAction.CallbackContext context)
        {
            var moveDirection = context.ReadValue<Vector2>();
            
            _movement.Value = new Vector3(moveDirection.x, 0f, moveDirection.y);
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    _jump.Value = true;
                    break;
                case InputActionPhase.Canceled:
                    _jump.Value = false;
                    break;
            }
        }

        public void OnRun(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    _run.Value = true;
                    break;
                case InputActionPhase.Canceled:
                    _run.Value = false;
                    break;
            }
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            
        }

        public void OnAim(InputAction.CallbackContext context)
        {
            
        }

        public void OnShoot(InputAction.CallbackContext context)
        {
            
        }

        public void Dispose()
        {
            _playerInput?.Disable();
            _playerInput?.Dispose();
            _movement?.Dispose();
            _jump?.Dispose();
            _run?.Dispose();
        }
    }
}