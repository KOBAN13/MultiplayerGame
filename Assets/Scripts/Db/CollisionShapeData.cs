using System;
using UnityEngine;
using Utils.Enums;

namespace Db
{
    [Serializable]
    public class CollisionShapeData
    {
        public string Name;
        public ECollisionShapeType Type;
            
        public int Layer;
        public string LayerName;
            
        public Quaternion Rotation;
        public Vector3 Scale;

        public Vector3 Center;
        public Vector3 Size;
        public float Radius;
        public float Height;
    }
}