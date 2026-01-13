using Db;
using Helpers;
using Player.Interface.Local;
using Sfs2X;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using UnityEngine;
using Utils.Enums;

namespace Player.Local
{
    public class PlayerColliderSender : IPlayerColliderSender
    {
        private readonly SmartFox _sfs;

        public PlayerColliderSender(SmartFox sfs)
        {
            _sfs = sfs;
        }

        //TODO: Сомнительно, после переделаю
        public void SendPlayerColliderToServer(Transform transform, int layer, CharacterController characterController)
        {
            var collisionMap = new CollisionShapeData();
            var result = SFSObject.NewInstance();
            
            collisionMap.Rotation = transform.rotation;
            collisionMap.Scale = transform.localScale;
            collisionMap.Name = characterController.gameObject.name;
            collisionMap.Type = ECollisionShapeType.Capsule;
            collisionMap.Layer = layer;
            collisionMap.LayerName = LayerMask.LayerToName(layer);
            collisionMap.Center = transform.TransformPoint(characterController.center);
            collisionMap.Radius = characterController.radius * Mathf.Max(Mathf.Abs(transform.lossyScale.x),
                Mathf.Abs(transform.lossyScale.y), Mathf.Abs(transform.lossyScale.z));
            collisionMap.Height = characterController.height;
            
            _sfs.Send(new ExtensionRequest(SFSResponseHelper.PLAYER_COLLIDER_DATA, result, _sfs.LastJoinedRoom));
        }
    }
}