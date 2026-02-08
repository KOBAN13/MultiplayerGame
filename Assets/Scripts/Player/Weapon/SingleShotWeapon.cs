using System;
using Db.Projectile;
using Helpers;
using Player.Weapon.Projectile;
using Sfs2X;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using UnityEngine;
using Utils.Enums;
using Utils.Pool;
using VContainer;

namespace Player.Weapon
{
    public class SingleShotWeapon : AWeapon
    {
        public override EWeaponType WeaponType => EWeaponType.SingleShot;
        
        private SmartFox _sfs;
        private IPoolService _poolService;
        private readonly AProjectile _projectile = new BulletProjectile();
        private Vector3 _lastShotRayOrigin;
        private Vector3 _lastShotRayDirection;
        private Vector3 _positionPlayerInShoot;

        private SingleShotWeaponData SingleShotWeaponData => (SingleShotWeaponData)Data;

        [Inject]
        private void Construct(SmartFox sfs, IPoolService poolService)
        {
            _sfs = sfs;
            _poolService = poolService;
        }
        
        protected override void PerformedAttack(ref FireCommand command)
        {
            command.shotData.layerMask = SingleShotWeaponData.LayerMask;
            command.shotData.distanceToShot = SingleShotWeaponData.DistanceToShot;
            
            SendShotToServer(command);
            SimulateShot(SingleShotWeaponData, command.shotData, command);
        }

        private void SendShotToServer(FireCommand command)
        {
            var shotData = command.shotData;
            
            var data = SFSObject.NewInstance();
            
            var originArray = new SFSArray();
            originArray.AddFloat(shotData.origin.x);
            originArray.AddFloat(shotData.origin.y);
            originArray.AddFloat(shotData.origin.z);
                
            var directionArray = new SFSArray();
            directionArray.AddFloat(shotData.direction.x);
            directionArray.AddFloat(shotData.direction.y);
            directionArray.AddFloat(shotData.direction.z);
            
            data.PutSFSArray("originVector", originArray);
            data.PutSFSArray("directionVector", directionArray);
            data.PutByte("shotAlpha", command.alpha);
            
            data.PutLong("snapshotId", command.snapshotId);
            data.PutInt("layerMask", shotData.layerMask);
            data.PutFloat("distance", shotData.distanceToShot);
                    
            _sfs.Send(new ExtensionRequest(SFSResponseHelper.RAYCAST, data, _sfs.LastJoinedRoom));
        }

        private void SimulateShot(SingleShotWeaponData weaponData, ShotData shotData, FireCommand command)
        {
            var distance = weaponData.DistanceToShot;

            if (Physics.Raycast(shotData.origin, 
                    shotData.direction, 
                    out var hit,
                    distance,
                    weaponData.LayerMask)
            )
            {
                var hitPoint = hit.point;
                
                var projectileData = SingleShotWeaponData.ProjectileData;
                    
                _projectile.InitializeProjectile(projectileData, _poolService);
                _projectile.Launch(EObjectInPoolName.BulletImpactEffect, hitPoint);
                
                Debug.LogError("Hit name: " + hit.collider.name);
                
                _lastShotRayOrigin = shotData.origin;
                _lastShotRayDirection = shotData.direction;
                _positionPlayerInShoot = command.position;

                Debug.DrawLine(_lastShotRayOrigin, hitPoint, Color.red, 1.0f);
            }
            else
            {
                Debug.DrawLine(_lastShotRayOrigin, _lastShotRayOrigin + _lastShotRayDirection * distance, Color.yellow, 1.0f);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(_positionPlayerInShoot, 0.2f);
        }
    }
}
