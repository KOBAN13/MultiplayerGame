using System;
using UnityEngine;

namespace Db
{
    [Serializable]
    public class PlayerCamerasParameters
    {
        public int CameraFOV { get; private set; }
        public Vector3 Damping { get; private set; }
        public Vector3 ShoulderOffset { get; private set; }
        public float CameraSide { get; private set; }
        public float CameraDistance { get; private set; }
    }
}