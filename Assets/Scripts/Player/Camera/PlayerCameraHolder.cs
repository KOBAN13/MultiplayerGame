using System.Collections.Generic;
using Db.Interface;
using Player.Interface;
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
            InitGameplayCamera(target);
        }

        private void InitGameplayCamera(Transform target)
        {
            if (_cameras.TryGetValue(EVirtualCameraType.Gameplay, out var camera))
            {
                camera.Priority = ACTIVE_PRIORITY;

                var cinemachineFollow = camera.gameObject.GetComponent<CinemachineFollow>();
                
                cinemachineFollow.FollowOffset = _cameraParameters.FollowOffset;
                cinemachineFollow.TrackerSettings.PositionDamping = _cameraParameters.PositionDamping;
                cinemachineFollow.TrackerSettings.BindingMode = _cameraParameters.BindingMode;
                
                AttachFollowTarget(camera,target);
            }
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
        
        private void InitFov()
        {
            foreach (var camera in _cameras.Values)
            {
                var lensSettings = camera.Lens;
                lensSettings.FieldOfView = _cameraParameters.CameraFOV;
                camera.Lens = lensSettings;
            }
        }
        
        private void AttachFollowTarget(CinemachineCamera camera, Transform followTarget)
        {
            camera.Target = new CameraTarget
            {
                CustomLookAtTarget = true,
                LookAtTarget = followTarget,
                TrackingTarget = followTarget
            };
        }
    }
}