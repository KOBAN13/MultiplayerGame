using Db.Projectile;
using UnityEngine;
using Utils.Enums;

namespace Player.Weapon
{
    public interface IWeapon
    {
        EWeaponType WeaponType { get; }
        void Attack();
        void Reload();
        AWeaponData GetWeaponData();
        void SetOwner(GameObject owner);
    }
}