using UnityEngine;

namespace Player.Interface
{
    public interface IRemotePlayer
    {
        Transform GetTransform();
        
        void SetAnimationState(string state);
        void SetSnapshot(Vector3 position, Vector3 rotationDirection, float serverTime);
    }
}