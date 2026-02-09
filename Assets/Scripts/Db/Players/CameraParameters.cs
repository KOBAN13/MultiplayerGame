using System.Collections.Generic;
using Db.Dictionary;
using Db.Interface;
using UnityEngine;
using Utils.Enums;

namespace Db.Players
{
    [CreateAssetMenu(fileName = "CameraParameters", menuName = "Db/CameraParameters")]
    public class CameraParameters : ScriptableObject, ICameraParameters
    {
        [SerializeField] private PlayerCameraParametersDictionary _cameraParametersByType = new();

        public IReadOnlyDictionary<EVirtualCameraType, PlayerCamerasParameters> CameraParametersByType
            => _cameraParametersByType;
    }
}
