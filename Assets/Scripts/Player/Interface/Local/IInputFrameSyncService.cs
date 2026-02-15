using Input;
using UnityEngine;

namespace Player.Interface.Local
{
    public interface IInputFrameSyncService
    {
        void Reset();
        void CaptureAndQueue(
            in InputFrame inputFrame,
            float deltaTime,
            Vector3 position,
            bool isGrounded,
            float rotationY,
            string animationState);
    }
}
