using Cysharp.Threading.Tasks;
using UnityEngine;
using Utils.Enums;

namespace Player.Weapon.Projectile
{
    public class BulletProjectile : AProjectile
    {
        private void OnCollisionEnter(Collision other) => OnHit(other);
        
        public override void OnHit(Collision collision)
        {
            if (collision.contactCount <= 0)
                return;
            
            var contact = collision.contacts[0];
                
            var collisionPoint = contact.point;
                
            ImpactEffect.transform.position = collisionPoint;
            
            ImpactEffect.gameObject.SetActive(true);
            
            ImpactEffect.Play();
            
            DestroyProjectile(EObjectInPoolName.BulletProjectile);

            UniTask.WaitUntil(() => !ImpactEffect.IsAlive())
                .ContinueWith(() => PoolService.ReturnToPool(EObjectInPoolName.BulletImpactEffect, ImpactEffect))
                .Forget();
        }

        // private float CalculateDamage(out bool isCrit, out float finalDamage)
        // {
        //     var baseDamage = Random.Range(Data.Damage / 1.5f, Data.Damage * 1.2f);
        //     isCrit = Random.value < _statsService.CriticalChance / 100f;
        //     finalDamage = isCrit ? baseDamage * _statsService.CriticalMultiplier : baseDamage;
        //     return baseDamage;
        // }
    }
}