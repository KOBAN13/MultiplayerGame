using Db.Projectile;
using Player.Weapon.Data;
using UnityEngine;
using Utils.Enums;

namespace Player.Weapon
{
    public interface IWeapon
    {
        EWeaponType WeaponType { get; }
        int GetWeaponId();
        void Attack(ref ServerFireCommand command);
        void Reload();
        AWeaponData GetWeaponData();
        void SetOwner(GameObject owner);
    }
}
