using System.Collections.Generic;
using Player.Camera;
using Unity.Cinemachine;
using UnityEngine;
using Utils.Enums;

namespace Player.Interface.Local
{
    public interface IPlayerCameraHolder
    {
        void Initialize(IReadOnlyDictionary<EVirtualCameraType, CinemachineCamera> cameraTypeData, Transform target);
        CinemachineCamera GetCurrentCamera();
        void SetVirtualCamera(EVirtualCameraType virtualCameraType);
    }
}