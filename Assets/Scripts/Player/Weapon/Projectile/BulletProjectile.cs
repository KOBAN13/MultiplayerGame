using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Utils.Enums;

namespace Player.Weapon.Projectile
{
    public class BulletProjectile : AProjectile
    {
        public override void OnHit(Vector3 hitPoint, Quaternion rotation)
        {
            if (PoolService.TrySpawn<ParticleSystem>(ImpactEffectId, true, hitPoint, out var impactEffect))
            {
                impactEffect.transform.rotation = rotation;
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
            
            DestroyProjectile(EObjectInPoolName.BulletImpactEffect);
        }

        public override void DestroyProjectile(EObjectInPoolName id)
        {
            //TODO: Может какой то эффект или еще чего
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
