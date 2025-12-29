using System.Collections.Generic;
using System.Linq;
using Player.Interface;
using Player.Interface.Local;
using Unity.Cinemachine;
using UnityEngine;
using Utils.Enums;
using VContainer;

namespace Player.Camera
{
    public class VirtualCameraView : MonoBehaviour
    {
        [SerializeField] private List<CameraTypeData> _cameraTypeDatas;

        private IPlayerCameraHolder _cameraHolder;
        
        [Inject]
        private void Construct(IPlayerCameraHolder cameraHolder)
        {
            _cameraHolder = cameraHolder;
        }

        public void InitializeCameras(Transform target)
        {
            var cameraTypeData = GetCameraTypeData();
            
            _cameraHolder.Initialize(cameraTypeData, target);
        }
        
        private IReadOnlyDictionary<EVirtualCameraType, CinemachineCamera> GetCameraTypeData()
        {
            return _cameraTypeDatas
                .ToDictionary(x => x.VirtualCameraType, x => x.VirtualCamera);
        }
    }
}