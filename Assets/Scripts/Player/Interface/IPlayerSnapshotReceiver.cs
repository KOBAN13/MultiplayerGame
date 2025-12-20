using Services.Interface;
using UnityEngine;

namespace Player.Interface
{
    public interface IPlayerSnapshotReceiver
    {
        void SetSnapshot(Vector3 position, Vector3 rotationDirection, float serverTime);
    }
}