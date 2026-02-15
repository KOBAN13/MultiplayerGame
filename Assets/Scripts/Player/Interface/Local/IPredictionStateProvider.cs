using Player.Db;
using UnityEngine;

namespace Player.Interface.Local
{
    public interface IPredictionStateProvider
    {
        void Write(Vector3 position, bool isGrounded, float rotation, string animationState, long inputTick);
        PredictionStateFrame Read();
    }
}