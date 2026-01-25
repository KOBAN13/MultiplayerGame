using UnityEngine;

namespace Player.Db
{
    public struct SnapshotData
    {
        public Vector3 Position;
        public Vector3 Input;
        public float Rotation;
        public long SnapshotId;
        public float ServerTime;
    }
}