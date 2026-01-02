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
        protected Rigidbody Rigidbody;
        protected EObjectInPoolName ImpactEffectId;
        
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
            Rigidbody.linearVelocity = Vector3.zero;
            Rigidbody.angularVelocity = Vector3.zero;
            Rigidbody.WakeUp();

            var directionEnd = direction.normalized;

            var rotation = Quaternion.LookRotation(directionEnd, Vector3.up);
            
            transform.rotation = rotation;
            Rigidbody.linearVelocity = directionEnd * speed;
            
            ImpactEffectId = impactEffectId;
            
            Debug.LogError("Rotation in launch: " + transform.rotation);
        }
        
        public virtual void DestroyProjectile(EObjectInPoolName id)
        {
            Rigidbody.linearVelocity = Vector3.zero;
            Rigidbody.angularVelocity = Vector3.zero;
            Rigidbody.Sleep();
            PoolService.ReturnToPool(id, this);
        }

        public AProjectileData GetData() => Data;
        public abstract void OnHit(Collision collision);
    }
}
