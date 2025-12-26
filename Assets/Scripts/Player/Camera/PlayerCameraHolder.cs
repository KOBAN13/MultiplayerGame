using System.Collections.Generic;
using Db.Interface;
using Player.Interface;
using Player.Interface.Local;
using Unity.Cinemachine;
using UnityEngine;

namespace Player.Camera
{
    public class PlayerCameraHolder : IPlayerCameraHolder
    {
        private readonly ICameraParameters _cameraParameters;
        
        private IReadOnlyDictionary<EVirtualCameraType, CinemachineCamera> _cameras;
        
        private const int ACTIVE_PRIORITY = 10;
        private const int INACTIVE_PRIORITY = 0;

        public PlayerCameraHolder(ICameraParameters cameraParameters)
        {
            _cameraParameters = cameraParameters;
        }

        public void Initialize(IReadOnlyDictionary<EVirtualCameraType, CinemachineCamera> cameraTypeData, Transform target)
        {
            _cameras = cameraTypeData;
            
            InitFov();
            InitCameras();
            
            if (_cameras.TryGetValue(EVirtualCameraType.Gameplay, out var camera))
            {
                AttachFollowTarget(camera, target);
            }
            
            SetVirtualCamera(EVirtualCameraType.Gameplay);
        }
        
        public CinemachineCamera GetCurrentCamera()
        {
            return _cameras?[EVirtualCameraType.Gameplay];
        }
        
        public void SetVirtualCamera(EVirtualCameraType virtualCameraType)
        {
            if (_cameras is null)
                return;

            foreach (var (type, camera) in _cameras)
            {
                var typesMatch = type == virtualCameraType;
                camera.Priority = typesMatch ? ACTIVE_PRIORITY : INACTIVE_PRIORITY;
            }
        }

        private void InitCameras()
        {
            foreach (var kayValueCamera in _cameras)
            {
                var cameraParameters = _cameraParameters.CameraParametersByType[kayValueCamera.Key];

                var camera = kayValueCamera.Value;
                
                camera.Priority = ACTIVE_PRIORITY;

                var cinemachineFollow = camera.gameObject.GetComponent<CinemachineThirdPersonFollow>();

                cinemachineFollow.Damping = cameraParameters.Damping;
                cinemachineFollow.ShoulderOffset = cameraParameters.ShoulderOffset;
                cinemachineFollow.CameraSide = cameraParameters.CameraSide;
                cinemachineFollow.CameraDistance = cameraParameters.CameraDistance;
            }
        }
        
        private void InitFov()
        {
            foreach (var camera in _cameras)
            {
                var cameraParameters = _cameraParameters.CameraParametersByType[camera.Key];
                
                var lensSettings = camera.Value.Lens;
                lensSettings.FieldOfView = cameraParameters.CameraFOV;
                camera.Value.Lens = lensSettings;
            }
        }
        
        private void AttachFollowTarget(CinemachineCamera camera, Transform followTarget)
        {
            camera.Target = new CameraTarget
            {
                TrackingTarget = followTarget
            };
        }
    }
}