using Db.Projectile;
using UnityEngine;
using Utils.Enums;

namespace Player.Weapon.Projectile
{
    public interface IProjectile
    {
        void OnHit(Vector3 hitPoint, Quaternion rotation);
        void DestroyProjectile(EObjectInPoolName id);
        AProjectileData GetData();
    }
}