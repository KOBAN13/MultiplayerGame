using Db.Interface;
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
using VContainer.Unity;

namespace Player.Shoot
{
    public class SimpleShotController : ISimpleShotController, IInitializable
    {
        private readonly SmartFox _sfs;
        private readonly IPoolService _poolService;
        private readonly IShotParameters _parameters;
        private readonly BulletData _bulletData;
        private Vector3 _lastShotRayOrigin;
        private Vector3 _lastShotRayDirection;
        private Vector3 _lastShotPointPosition;
        private bool _hasPendingShot;

        public SimpleShotController(
            SmartFox sfs,
            IPoolService poolService,
            IShotParameters parameters,
            BulletData bulletData)
        {
            _sfs = sfs;
            _poolService = poolService;
            _parameters = parameters;
            _bulletData = bulletData;
        }

        public void Initialize()
        {
            _sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, HandlerRaycast);
        }

        public void Shot(Transform shotPoint)
        {
            var screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
            var camera = UnityEngine.Camera.main;

            if (camera == null) 
                return;
            
            var ray = camera.ScreenPointToRay(screenCenterPoint);
            _lastShotRayOrigin = ray.origin;
            _lastShotRayDirection = ray.direction;
            _lastShotPointPosition = shotPoint.position;
            _hasPendingShot = true;
                
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
            data.PutInt("layerMask", _parameters.LayerMask);
            data.PutFloat("distance", _parameters.DistanceToShot);
                    
            _sfs.Send(new ExtensionRequest(SFSResponseHelper.RAYCAST, data, _sfs.LastJoinedRoom));
        }

        private void HandlerRaycast(BaseEvent evt)
        {
            var cmd = (string)evt.Params[SFSResponseHelper.CMD];
            
            if (cmd != SFSResponseHelper.RAYCAST)
                return;
            if (!_hasPendingShot)
                return;

            var sfsObject = (SFSObject)evt.Params["params"];
            
            var hit = sfsObject.GetBool("hit");
            var distance = sfsObject.ContainsKey("distance")
                ? sfsObject.GetFloat("distance")
                : _parameters.DistanceToShot;

            if (hit && sfsObject.ContainsKey("x") && sfsObject.ContainsKey("y") && sfsObject.ContainsKey("z"))
            {
                var x = sfsObject.GetFloat("x");
                var y = sfsObject.GetFloat("y");
                var z = sfsObject.GetFloat("z");
                var hitPoint = new Vector3(x, y, z);
                var direction = hitPoint - _lastShotPointPosition;

                if (direction.sqrMagnitude > 0.0001f && _poolService.TrySpawn<AProjectile>(
                        EObjectInPoolName.BulletProjectile, true, _lastShotPointPosition, out var projectile))
                {
                    projectile.InitializeProjectile(_bulletData, _poolService);
                    projectile.Launch(EObjectInPoolName.BulletImpactEffect, direction, _bulletData.Speed);
                }

                Debug.DrawLine(_lastShotRayOrigin, hitPoint, Color.red, 1.0f);
            }
            else
            {
                Debug.DrawLine(_lastShotRayOrigin, _lastShotRayOrigin + _lastShotRayDirection * distance, Color.yellow, 1.0f);
            }

            _hasPendingShot = false;
        }
    }
}
