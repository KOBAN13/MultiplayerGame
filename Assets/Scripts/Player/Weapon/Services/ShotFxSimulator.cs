using Db.Projectile;
using Player.Weapon.Data;
using Player.Weapon.Projectile;
using UnityEngine;
using Utils.Enums;
using Utils.Pool;

namespace Player.Weapon.Services
{
    public class ShotFxSimulator : IShotFxSimulator
    {
        private readonly IPoolService _poolService;
        private readonly AProjectile _projectile = new BulletProjectile();

        public ShotFxSimulator(IPoolService poolService)
        {
            _poolService = poolService;
        }

        public void SimulateShotClient(AHitScanWeaponData weaponData, in ShotData shotData)
        {
            if (weaponData == null)
                return;

            var distance = weaponData.ShotDistance;
            var impactEffectId = ResolveImpactEffect(weaponData);

            if (Physics.Raycast(shotData.origin, 
                    shotData.direction, 
                    out var hit,
                    distance,
                    weaponData.ShotLayerMask)
               )
            {
                var hitPoint = hit.point;
                
                var projectileData = weaponData.ProjectileData;
                    
                _projectile.InitializeProjectile(projectileData, _poolService);
                _projectile.Launch(impactEffectId, hitPoint);
                Debug.DrawLine(shotData.origin, hitPoint, Color.red, 1.0f);
            }
            else
            {
                Debug.DrawLine(shotData.origin, shotData.origin + shotData.direction * distance, Color.yellow, 1.0f);
            }
        }
        
        public void SimulateShotServer(AHitScanWeaponData weaponData, in ClientFireCommand shotData)
        {
            if (weaponData == null || !shotData.isHit)
                return;

            var hitPoint = shotData.hitPosition;
                
            var projectileData = weaponData.ProjectileData;
            var impactEffectId = ResolveImpactEffect(weaponData);
                    
            _projectile.InitializeProjectile(projectileData, _poolService);
            _projectile.Launch(impactEffectId, hitPoint);
        }

        private static EObjectInPoolName ResolveImpactEffect(AHitScanWeaponData weaponData)
        {
            return weaponData.ImpactEffectId == EObjectInPoolName.None
                ? EObjectInPoolName.BulletImpactEffect
                : weaponData.ImpactEffectId;
        }
    }
}
