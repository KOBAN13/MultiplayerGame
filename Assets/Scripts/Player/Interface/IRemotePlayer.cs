using UnityEngine;

namespace Player.Interface
{
    public interface IRemotePlayer
    {
        Transform GetTransform();
        Transform GetCameraTarget();
        
        void SetAnimationState(string state);
        void SetSnapshot(Vector3 position, Vector3 rotationDirection, float serverTime);
    }
}