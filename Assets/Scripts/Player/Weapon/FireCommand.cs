using UnityEngine;

namespace Player.Weapon
{
    public struct FireCommand
    {
        public int weaponId;
        public Vector3 position;
        public byte alpha;
        public long snapshotId;
        public ShotData shotData;
    }
}