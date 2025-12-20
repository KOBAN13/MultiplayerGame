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
        private readonly ReactiveProperty<Vector2> _look = new();
        private readonly ReactiveProperty<bool> _aim = new();
        private readonly ReactiveProperty<bool> _shoot = new();
        private readonly ReactiveProperty<bool> _jump = new();
        private readonly ReactiveProperty<bool> _run = new();
        
        public ReadOnlyReactiveProperty<Vector3> Movement => _movement;
        public ReadOnlyReactiveProperty<bool> Jump => _jump;
        public ReadOnlyReactiveProperty<bool> Run => _run;
        public ReadOnlyReactiveProperty<Vector2> Look => _look;
        public ReadOnlyReactiveProperty<bool> Aim => _aim;
        public ReadOnlyReactiveProperty<bool> Shoot => _shoot;
        
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
            _look.Value = context.ReadValue<Vector2>();
        }

        public void OnAim(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    _aim.Value = true;
                    break;
                case InputActionPhase.Canceled:
                    _aim.Value = false;
                    break;
            }
        }

        public void OnShoot(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    _shoot.Value = true;
                    break;
                case InputActionPhase.Canceled:
                    _aim.Value = false;
                    break;
            }
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