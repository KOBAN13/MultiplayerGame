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
        private readonly SmartFox _sfs;
        private readonly CharacterController _characterController;
        private readonly Transform _cameraTransform;
        private IDisposable _subscription;
        
        public PlayerNetworkInputSender(
            SmartFox sfs,
            CharacterController characterController, Transform cameraTransform
        )
        {
            _sfs = sfs;
            _characterController = characterController;
            _cameraTransform = cameraTransform;
        }

        public void Dispose()
        {
            _subscription?.Dispose();
        }

        public void SendServerPlayerInput(InputFrame inputFrame)
        {
            var data = SFSObject.NewInstance();
            data.PutFloat("horizontal", inputFrame.Movement.x);
            data.PutFloat("vertical", inputFrame.Movement.z);
            data.PutBool("isJumping", inputFrame.Jump);
            data.PutBool("isRunning", inputFrame.Run);
            data.PutBool("isOnGround", _characterController.isGrounded);
            data.PutFloat("eulerAngleY", _cameraTransform.eulerAngles.y);
                    
            _sfs.Send(new ExtensionRequest(SFSResponseHelper.PLAYER_INPUT, data, _sfs.LastJoinedRoom));
        }
    }
}
