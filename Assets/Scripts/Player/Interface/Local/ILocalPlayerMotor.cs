using UnityEngine;

namespace Player.Interface.Local
{
    public interface ILocalPlayerMotor
    {
        Transform GetTransform();
        Transform GetCameraTarget();
    }
}