using UnityEngine;

namespace Player.Interface
{
    public interface IRemotePlayer
    {
        void SetAnimationState(string state);
        void SetSnapshot(Vector3 position, Vector3 inputDirection, float rotation, float serverTime);
    }
}