using UnityEngine;

namespace Player.Interface
{
    public interface IRotationComponent
    {
        void RotateCharacter();
        Quaternion RotateCamera(Vector2 position);
    }
}