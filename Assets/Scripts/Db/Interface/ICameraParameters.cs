using System.Collections.Generic;
using Db.Players;
using Utils.Enums;

namespace Db.Interface
{
    public interface ICameraParameters
    {
        IReadOnlyDictionary<EVirtualCameraType, PlayerCamerasParameters> CameraParametersByType { get; }
    }
}
