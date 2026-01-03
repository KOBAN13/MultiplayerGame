using System;
using Db.Projectile;
using UnityEngine;
using Utils.Enums;
using Utils.Pool;

namespace Player.Weapon.Projectile
{
    public abstract class AProjectile : IProjectile
    {
        protected AProjectileData Data;
        protected IPoolService PoolService;
        protected EObjectInPoolName ImpactEffectId;
        
        public virtual void InitializeProjectile(AProjectileData data, IPoolService poolService)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            
            Data = data;
            PoolService = poolService;
        }

        public virtual void Launch(EObjectInPoolName impactEffectId, Vector3 hitPoint)
        {
            var directionEnd = hitPoint.normalized;

            var rotation = Quaternion.LookRotation(directionEnd, Vector3.up);
            
            ImpactEffectId = impactEffectId;
            
            OnHit(hitPoint, rotation);
        }

        public abstract void DestroyProjectile(EObjectInPoolName id);

        public AProjectileData GetData() => Data;

        public abstract void OnHit(Vector3 hitPoint, Quaternion rotation);
    }
}
