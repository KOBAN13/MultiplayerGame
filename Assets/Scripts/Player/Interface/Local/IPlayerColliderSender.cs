using UnityEngine;

namespace Player.Interface.Local
{
    public interface IPlayerColliderSender
    {
        void SendPlayerColliderToServer(Transform transform, int layer, CharacterController characterController);
    }
}