using System;
using Helpers;
using Input;
using Player.Db;
using Player.Interface.Local;
using Sfs2X;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;

namespace Player.Local
{
    public class PlayerNetworkStateSender : IPlayerNetworkStateSender
    {
        private readonly SmartFox _sfs;
        private IDisposable _subscription;
        
        public PlayerNetworkStateSender(SmartFox sfs)
        {
            _sfs = sfs;
        }

        public void Dispose()
        {
            _subscription?.Dispose();
        }

        public void SendServerPlayerState(InputFrame inputFrame)
        {
            var data = SFSObject.NewInstance();
            data.PutFloat("horizontal", inputFrame.Movement.x);
            data.PutFloat("vertical", inputFrame.Movement.z);
            data.PutBool("isJumping", inputFrame.Jump);
            data.PutBool("isRunning", inputFrame.Run);
                    
            _sfs.Send(new ExtensionRequest(SFSResponseHelper.PLAYER_INPUT, data, _sfs.LastJoinedRoom));
        }
        
        public void SendServerPlayerInput(ClientStateFrame stateFrame)
        {
            var data = SFSObject.NewInstance();
            data.PutBool("isOnGround", stateFrame.IsGrounded);
            data.PutFloat("eulerAngleY", stateFrame.RotationY);
            data.PutFloat("targetDirX", stateFrame.AimDirection.x);
            data.PutFloat("targetDirY", stateFrame.AimDirection.y);
            data.PutFloat("targetDirZ", stateFrame.AimDirection.z);
            data.PutFloat("targetPitch", stateFrame.AimPitch);
                    
            _sfs.Send(new ExtensionRequest(SFSResponseHelper.PLAYER_CLIENT_STATE, data, _sfs.LastJoinedRoom));
        }
    }
}
