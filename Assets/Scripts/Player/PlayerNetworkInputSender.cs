using System;
using Helpers;
using Input;
using Player.Interface;
using Sfs2X;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using UnityEngine;

namespace Player
{
    public class PlayerNetworkInputSender : IPlayerNetworkInputSender
    {
        private readonly IPlayerNetworkInputReader _playerNetworkInputReader;
        private readonly SmartFox _sfs;
        private readonly CharacterController _characterController;
        private readonly Transform _cameraTransform;
        private IDisposable _subscription;
        
        public PlayerNetworkInputSender(
            IPlayerNetworkInputReader playerNetworkInputReader,
            SmartFox sfs,
            CharacterController characterController, Transform cameraTransform
        )
        {
            _playerNetworkInputReader = playerNetworkInputReader;
            _sfs = sfs;
            _characterController = characterController;
            _cameraTransform = cameraTransform;

            Initialize();
        }

        public void Dispose()
        {
            _subscription?.Dispose();
        }

        private void Initialize()
        {
            // _subscription = Observable
            //     .Merge(
            //         _playerNetworkInputReader.Movement.AsUnitObservable(),
            //         _playerNetworkInputReader.Jump.AsUnitObservable(),
            //         _playerNetworkInputReader.Run.AsUnitObservable()
            //     )
            //     .Subscribe(SendServerPlayerInput);
        }

        public void SendServerPlayerInput()
        {
            var data = SFSObject.NewInstance();
            data.PutFloat("horizontal", _playerNetworkInputReader.Movement.CurrentValue.x);
            data.PutFloat("vertical", _playerNetworkInputReader.Movement.CurrentValue.z);
            data.PutBool("isJumping", _playerNetworkInputReader.Jump.CurrentValue);
            data.PutBool("isRunning", _playerNetworkInputReader.Run.CurrentValue);
            data.PutBool("isOnGround", _characterController.isGrounded);
            data.PutFloat("eulerAngleY", _cameraTransform.eulerAngles.y);
                    
            _sfs.Send(new ExtensionRequest(SFSResponseHelper.PLAYER_INPUT, data, _sfs.LastJoinedRoom));
        }
    }
}