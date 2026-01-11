using Input;
using Player.Db;
using UnityEngine;

namespace Player.Interface.Local
{
    public interface IClientStateProvider
    {
        void Write(float rotationCameraY, Vector3 aimDirection, float aimPitch);
        ClientStateFrame Read(InputFrame inputFrame);
    }
}
