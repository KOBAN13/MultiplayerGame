using UnityEngine;

namespace Player.Weapon
{
    public struct FireCommand
    {
        public int weaponId;
        public byte alpha;
        public long snapshotId;
        public Vector3 position;
        public ShotData shotData;
    }
}