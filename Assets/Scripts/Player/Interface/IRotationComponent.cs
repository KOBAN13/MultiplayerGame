using UnityEngine;

namespace Player.Interface
{
    public interface IRotationComponent
    {
        void RotateCharacter(Transform cameraTransform);
        Quaternion RotateCamera(Vector2 position);
    }
}