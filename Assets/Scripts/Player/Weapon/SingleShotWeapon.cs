using Db.Projectile;
using Helpers;
using Player.Weapon.Projectile;
using Sfs2X;
using Sfs2X.Core;
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
        [Header("References")]
        [SerializeField] private Transform _shotPoint;
        
        public override EWeaponType WeaponType => EWeaponType.SingleShot;
        
        private SmartFox _sfs;
        private IPoolService _poolService;
        private readonly AProjectile _projectile = new BulletProjectile();
        private Vector3 _lastShotRayOrigin;
        private Vector3 _lastShotRayDirection;
        
        private SingleShotWeaponData SingleShotWeaponData => (SingleShotWeaponData)Data;

        [Inject]
        private void Construct(SmartFox sfs, IPoolService poolService)
        {
            _sfs = sfs;
            _poolService = poolService;
            
            _sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, HandlerRaycast);
        }
        
        protected override void PerformedAttack()
        {
            Shot(SingleShotWeaponData);
        }
        
        private void Shot(SingleShotWeaponData weaponData)
        {
            var screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
            
            var main = UnityEngine.Camera.main;

            if (main == null) 
                return;
            
            var ray = main.ScreenPointToRay(screenCenterPoint);
            _lastShotRayOrigin = ray.origin;
            _lastShotRayDirection = ray.direction;
                
            var data = SFSObject.NewInstance();
            
            var direction = ray.direction;
            var origin = ray.origin;
                
            var originArray = new SFSArray();
            originArray.AddFloat(origin.x);
            originArray.AddFloat(origin.y);
            originArray.AddFloat(origin.z);
                
            var directionArray = new SFSArray();
            directionArray.AddFloat(direction.x);
            directionArray.AddFloat(direction.y);
            directionArray.AddFloat(direction.z);
                
            data.PutSFSArray("originVector", originArray);
            data.PutSFSArray("directionVector", directionArray);
            data.PutInt("layerMask", weaponData.LayerMask);
            data.PutFloat("distance", weaponData.DistanceToShot);
                    
            _sfs.Send(new ExtensionRequest(SFSResponseHelper.RAYCAST, data, _sfs.LastJoinedRoom));
        }

        private void HandlerRaycast(BaseEvent evt)
        {
            var cmd = (string)evt.Params[SFSResponseHelper.CMD];
            
            if (cmd != SFSResponseHelper.RAYCAST)
                return;

            var sfsObject = (SFSObject)evt.Params["params"];
            
            var hit = sfsObject.GetBool("hit");
            var distance = sfsObject.ContainsKey("distance")
                ? sfsObject.GetFloat("distance")
                : SingleShotWeaponData.DistanceToShot;

            if (hit)
            {
                var xPoint = sfsObject.GetFloat("xPoint");
                var yPoint = sfsObject.GetFloat("yPoint");
                var zPoint = sfsObject.GetFloat("zPoint");
                
                var hitPoint = new Vector3(xPoint, yPoint, zPoint);
                
                var projectileData = SingleShotWeaponData.ProjectileData;
                    
                _projectile.InitializeProjectile(projectileData, _poolService);
                _projectile.Launch(EObjectInPoolName.BulletImpactEffect, hitPoint);

                Debug.DrawLine(_lastShotRayOrigin, hitPoint, Color.red, 1.0f);
            }
            else
            {
                Debug.DrawLine(_lastShotRayOrigin, _lastShotRayOrigin + _lastShotRayDirection * distance, Color.yellow, 1.0f);
            }
        }
    }
}
