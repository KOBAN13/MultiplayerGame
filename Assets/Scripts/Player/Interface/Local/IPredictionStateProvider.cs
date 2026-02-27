using Player.Db;
using UnityEngine;

namespace Player.Interface.Local
{
    public interface IPredictionStateProvider
    {
        void Write(
            Vector3 position,
            Vector3 velocity,
            Vector3 moveDirection,
            bool isGrounded,
            float rotation,
            int animationState,
            long inputTick);

        PredictionStateFrame Read();
    }
}
