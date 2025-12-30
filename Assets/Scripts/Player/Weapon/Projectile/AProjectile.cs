using System;
using Db.Projectile;
using UnityEngine;
using Utils.Enums;
using Utils.Pool;

namespace Player.Weapon.Projectile
{
    public abstract class AProjectile : MonoBehaviour, IProjectile
    {
        protected AProjectileData Data;
        protected IPoolService PoolService;
        protected ParticleSystem ImpactEffect;
        protected Rigidbody Rigidbody;
        
        public virtual void InitializeProjectile(AProjectileData data, IPoolService poolService)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            
            Data = data;
            PoolService = poolService;
            Rigidbody = GetComponent<Rigidbody>();
        }
        
        public virtual void Launch(EObjectInPoolName impactEffectId, Vector3 direction, float speed)
        { 
            Rigidbody.WakeUp();
            
            var directionEnd = direction.normalized;

            transform.forward = directionEnd;
            
            Rigidbody.linearVelocity = directionEnd * speed;
            
            if(PoolService.TrySpawn<ParticleSystem>(impactEffectId, false, out var impactEffect))
            {
                ImpactEffect = impactEffect;
            }
        }
        
        public virtual void DestroyProjectile(EObjectInPoolName id)
        {
            Rigidbody.Sleep();
            PoolService.ReturnToPool(id, this);
        }

        public AProjectileData GetData() => Data;
        public abstract void OnHit(Collision collision);
    }
}