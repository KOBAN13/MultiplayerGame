#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Editor
{
    public partial class CollisionMap
    {
        [Serializable]
        public class CollisionMapPayload
        {
            public string SceneName;
            public List<CollisionShapeData> Shapes = new();
        }
        
        [Serializable]
        public enum ECollisionShapeType
        {
            None = 0,
            Box = 1,
            Sphere = 2,
            Capsule = 3,
            Mesh = 4,
            Terrain = 5
        }
        
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
            public int Direction;
        }
    }
}
#endif
