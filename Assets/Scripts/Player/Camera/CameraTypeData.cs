using System;
using Unity.Cinemachine;
using Utils.Enums;

namespace Player.Camera
{
    [Serializable]
    public struct CameraTypeData
    {
        public EVirtualCameraType VirtualCameraType;
        public CinemachineCamera VirtualCamera;
    }
}