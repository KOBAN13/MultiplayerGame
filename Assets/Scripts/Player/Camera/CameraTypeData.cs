using System;
using Unity.Cinemachine;

namespace Player.Camera
{
    [Serializable]
    public struct CameraTypeData
    {
        public EVirtualCameraType VirtualCameraType;
        public CinemachineCamera VirtualCamera;
    }
}