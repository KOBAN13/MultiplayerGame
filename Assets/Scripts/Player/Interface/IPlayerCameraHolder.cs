using System.Collections.Generic;
using Player.Camera;
using Unity.Cinemachine;
using UnityEngine;

namespace Player.Interface
{
    public interface IPlayerCameraHolder
    {
        void Initialize(IReadOnlyDictionary<EVirtualCameraType, CinemachineCamera> cameraTypeData, Transform target);
    }
}