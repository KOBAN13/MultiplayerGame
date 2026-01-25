using Db.Projectile;
using UnityEngine;
using Utils.Enums;

namespace Player.Weapon
{
    public interface IWeapon
    {
        EWeaponType WeaponType { get; }
        int GetWeaponId();
        void Attack(ref FireCommand command);
        void Reload();
        AWeaponData GetWeaponData();
        void SetOwner(GameObject owner);
    }
}
