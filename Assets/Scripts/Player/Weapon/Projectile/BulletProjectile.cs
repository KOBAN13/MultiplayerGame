using System;
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
            
            Debug.LogError("Rotation in hit: " + transform.rotation);
            
            if (PoolService.TrySpawn<ParticleSystem>(ImpactEffectId, true, collisionPoint, out var impactEffect))
            {
                impactEffect.transform.rotation = Quaternion.LookRotation(contact.normal);
                impactEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                impactEffect.Clear(true);
                impactEffect.Play();

                var main = impactEffect.main;
                var totalLifetime = main.duration + main.startLifetime.constantMax;

                UniTask.Delay(TimeSpan.FromSeconds(totalLifetime),
                        cancellationToken: impactEffect.GetCancellationTokenOnDestroy())
                    .ContinueWith(() => PoolService.ReturnToPool(EObjectInPoolName.BulletImpactEffect, impactEffect))
                    .Forget();
            }
            
            DestroyProjectile(EObjectInPoolName.BulletProjectile);
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
