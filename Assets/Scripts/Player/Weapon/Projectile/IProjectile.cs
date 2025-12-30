using Db.Projectile;
using UnityEngine;
using Utils.Enums;

namespace Player.Weapon.Projectile
{
    public interface IProjectile
    {
        void Launch(EObjectInPoolName impactEffectId, Vector3 direction, float speed);
        void OnHit(Collision collision);
        void DestroyProjectile(EObjectInPoolName id);
        AProjectileData GetData();
    }
}