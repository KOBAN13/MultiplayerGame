using Db.Interface;
using Helpers;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using UnityEngine;
using VContainer.Unity;

namespace Player.Shoot
{
    public class SimpleShotController : ISimpleShotController, IInitializable
    {
        private readonly SmartFox _sfs;
        private readonly IShotParameters  _parameters;
        private Vector3 _lastShotRayOrigin;
        private Vector3 _lastShotRayDirection;

        public SimpleShotController(SmartFox sfs, IShotParameters parameters)
        {
            _sfs = sfs;
            _parameters = parameters;
        }

        public void Initialize()
        {
            _sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, HandlerRaycast);
        }

        public void Shot()
        {
            var screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
            var camera = UnityEngine.Camera.main;

            if (camera == null) 
                return;
            
            var ray = camera.ScreenPointToRay(screenCenterPoint);
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
            data.PutInt("layerMask", _parameters.LayerMask);
            data.PutFloat("distance", _parameters.DistanceToShot);
                    
            _sfs.Send(new ExtensionRequest(SFSResponseHelper.RAYCAST, data, _sfs.LastJoinedRoom));
        }

        private void HandlerRaycast(BaseEvent evt)
        {
            var cmd = (string)evt.Params[SFSResponseHelper.CMD];
            
            if (cmd != SFSResponseHelper.RAYCAST)
                return;

            var sfsObject = (SFSObject)evt.Params["params"];
            
            var hit = sfsObject.GetBool("hit");
            var distance = sfsObject.GetFloat("distance");
            
            var x = sfsObject.GetFloat("x");
            var y = sfsObject.GetFloat("y");
            var z = sfsObject.GetFloat("z");
            
            var hitPoint = new Vector3(x, y, z);
            
            if (hit)
            {
                Debug.DrawLine(_lastShotRayOrigin, hitPoint, Color.red, 1.0f);
            }
            else
            {
                Debug.DrawLine(_lastShotRayOrigin, _lastShotRayOrigin + _lastShotRayDirection * distance, Color.yellow, 1.0f);
            }
        }
    }
}
