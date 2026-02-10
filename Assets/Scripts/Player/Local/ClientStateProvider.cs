using Input;
using Player.Db;
using Player.Interface.Local;
using UnityEngine;

namespace Player.Local
{
    public class ClientStateProvider : IClientStateProvider
    {
        private float _rotationCameraY;
        private Vector3 _aimDirection;
        private float _aimPitch;

        public void Write(float rotationCameraY, Vector3 aimDirection, float aimPitch)
        {
            _rotationCameraY = rotationCameraY;
            _aimDirection = aimDirection;
            _aimPitch = aimPitch;
        }
        
        public ClientStateFrame Read(InputFrame inputFrame)
        {
            return new ClientStateFrame()
            {
                RotationY = _rotationCameraY,
                AimDirection = _aimDirection,
                AimPitch = _aimPitch,
            };
        }
    }
}