using System;
using UnityEngine;

namespace Db
{
    [Serializable]
    public class PlayerCamerasParameters
    {
        [SerializeField] private int _cameraFov;
        [SerializeField] private Vector3 _damping;
        [SerializeField] private Vector3 _shoulderOffset;
        [SerializeField, Range(0, 1)] private float _cameraSide;
        [SerializeField] private float _cameraDistance;

        public int CameraFOV => _cameraFov;
        public Vector3 Damping => _damping;
        public Vector3 ShoulderOffset => _shoulderOffset;
        public float CameraSide => _cameraSide;
        public float CameraDistance => _cameraDistance;
    }
}
