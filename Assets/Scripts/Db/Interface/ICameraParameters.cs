using System.Collections.Generic;
using Player.Camera;

namespace Db.Interface
{
    public interface ICameraParameters
    {
        IReadOnlyDictionary<EVirtualCameraType, PlayerCamerasParameters> CameraParametersByType { get; }
    }
}
