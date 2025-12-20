using System;
using Helpers;
using Input;
using Player.Interface;
using R3;
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
        private IDisposable _subscription;
        
        public PlayerNetworkInputSender(
            IPlayerNetworkInputReader playerNetworkInputReader,
            SmartFox sfs,
            CharacterController characterController
        )
        {
            _playerNetworkInputReader = playerNetworkInputReader;
            _sfs = sfs;
            _characterController = characterController;
            
            Initialize();
        }

        public void Dispose()
        {
            _subscription?.Dispose();
        }

        private void Initialize()
        {
            _subscription = Observable
                .Merge(
                    _playerNetworkInputReader.Movement.AsUnitObservable(),
                    _playerNetworkInputReader.Jump.AsUnitObservable(),
                    _playerNetworkInputReader.Run.AsUnitObservable()
                )
                .Subscribe(SendServerPlayerInput);
        }

        private void SendServerPlayerInput(Unit unit)
        {
            var data = SFSObject.NewInstance();
            data.PutFloat("horizontal", _playerNetworkInputReader.Movement.CurrentValue.x);
            data.PutFloat("vertical", _playerNetworkInputReader.Movement.CurrentValue.z);
            data.PutBool("isJumping", _playerNetworkInputReader.Jump.CurrentValue);
            data.PutBool("isRunning", _playerNetworkInputReader.Run.CurrentValue);
            data.PutBool("isOnGround", _characterController.isGrounded);
                    
            _sfs.Send(new ExtensionRequest(SFSResponseHelper.PLAYER_INPUT, data, _sfs.LastJoinedRoom));
        }
    }
}