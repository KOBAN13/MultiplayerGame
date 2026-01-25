using UnityEngine;

namespace Player.Weapon
{
    public struct ShotData
    {
        public Vector3 origin;
        public Vector3 direction;
        public int layerMask;
        public float distanceToShot;
    }
}