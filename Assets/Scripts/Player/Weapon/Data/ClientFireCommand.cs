using UnityEngine;

namespace Player.Weapon.Data
{
    public struct ClientFireCommand
    {
        public int weaponId;
        public bool isHit;
        public Vector3 hitPosition;
    }
}