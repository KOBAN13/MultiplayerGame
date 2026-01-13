#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Db;
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
    }
}
#endif
