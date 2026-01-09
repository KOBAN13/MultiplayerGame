using System.Collections.Generic;
using Helpers;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities.Data;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using VContainer;

namespace DebugUtils
{
    #if UNITY_EDITOR
    public class DrawServerShape : SerializedMonoBehaviour
    {
        private SmartFox _sfs;
        [OdinSerialize] private Dictionary<string, ShapeData> _shapeData = new();

        [Inject]
        private void Construct(SmartFox sfs)
        {
            _sfs = sfs;
        }

        private void Awake()
        {
            _sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnDrawServerShape);
        }

        private void OnDrawServerShape(BaseEvent evt)
        {
            var cmd = (string)evt.Params[SFSResponseHelper.CMD];
            
            if (cmd != SFSResponseHelper.COLLISION_DATA)
                return;
            
            var data = (SFSObject)evt.Params["params"];
            
            var shapeArray = data.GetSFSArray("shapesData");

            for (var i = 0; i < shapeArray.Count; i++)
            {
                var sfsData = shapeArray.GetSFSObject(i);
                
                var shapeId = sfsData.GetUtfString("shapeId");
                var hasObb = sfsData.GetBool("hasObb");

                if (hasObb)
                {
                    var obbCx = sfsData.GetFloat("obbCx");
                    var obbCy = sfsData.GetFloat("obbCy");
                    var obbCz = sfsData.GetFloat("obbCz");

                    var obbHx = sfsData.GetFloat("obbHx");
                    var obbHy = sfsData.GetFloat("obbHy");
                    var obbHz = sfsData.GetFloat("obbHz");
                    
                    var obbRotationX = sfsData.GetFloat("obbQx");
                    var obbRotationY = sfsData.GetFloat("obbQy");
                    var obbRotationZ = sfsData.GetFloat("obbQz");
                    var obbRotationW = sfsData.GetFloat("obbQw");
                    
                    _shapeData.Add(shapeId, new ShapeData()
                    {
                        HasObb = true,
                        ObbC = new Vector3(obbCx, obbCy, obbCz),
                        ObbH = new  Vector3(obbHx, obbHy, obbHz),
                        ObbRotation = new Quaternion(obbRotationX, obbRotationY, obbRotationZ, obbRotationW),
                        Min = Vector3.zero,
                        Max = Vector3.zero,
                    });
                }
                else
                {
                    var minX = sfsData.GetFloat("minX");
                    var minY = sfsData.GetFloat("minY");
                    var minZ = sfsData.GetFloat("minZ");

                    var maxX = sfsData.GetFloat("maxX");
                    var maxY = sfsData.GetFloat("maxY");
                    var maxZ = sfsData.GetFloat("maxZ");
                    
                    _shapeData.Add(shapeId, new ShapeData()
                    {
                        HasObb = false,
                        ObbC = Vector3.zero,
                        ObbH = Vector3.zero,
                        ObbRotation = Quaternion.identity,
                        Min = new Vector3(minX, minY, minZ),
                        Max = new Vector3(maxX, maxY, maxZ),
                    });
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (_shapeData == null || _shapeData.Count == 0)
            {
                return;
            }

            var originalMatrix = Gizmos.matrix;
            Gizmos.color = Color.cyan;

            foreach (var shape in _shapeData.Values)
            {
                if (shape.HasObb)
                {
                    var rotation = SafeQuaternion(shape.ObbRotation);
                    Gizmos.matrix = Matrix4x4.TRS(shape.ObbC, rotation, Vector3.one);
                    Gizmos.DrawWireCube(Vector3.zero, shape.ObbH * 2f);
                }
                else
                {
                    var center = (shape.Min + shape.Max) * 0.5f;
                    var size = shape.Max - shape.Min;
                    Gizmos.matrix = Matrix4x4.identity;
                    Gizmos.DrawWireCube(center, size);
                }
            }

            Gizmos.matrix = originalMatrix;
        }
        
        private static Quaternion SafeQuaternion(Quaternion q)
        {
            if (float.IsNaN(q.x) || float.IsNaN(q.y) || float.IsNaN(q.z) || float.IsNaN(q.w) ||
                float.IsInfinity(q.x) || float.IsInfinity(q.y) || float.IsInfinity(q.z) || float.IsInfinity(q.w))
            {
                return Quaternion.identity;
            }
            
            var magSq = q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w;
            if (magSq < 1e-12f)
                return Quaternion.identity;

            var invMag = 1.0f / Mathf.Sqrt(magSq);
            q.x *= invMag;
            q.y *= invMag;
            q.z *= invMag;
            q.w *= invMag;

            return q;
        }

        
        private struct ShapeData
        {
            public bool HasObb;
            public Vector3 ObbC;
            public Vector3 ObbH;
            public Quaternion ObbRotation;
            public Vector3 Min;
            public Vector3 Max;
        }
    }
    
    #endif
}
