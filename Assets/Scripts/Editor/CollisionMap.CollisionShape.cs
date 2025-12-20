#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Editor
{
    public partial class CollisionMap
    {
        private CollisionMapPayload CollectFromScene(Scene scene)
        {
            var payload = new CollisionMapPayload
            {
                SceneName = scene.name,
            };
            
            var roots = scene.GetRootGameObjects();
            
            foreach (var root in roots)
            {
                foreach (var collider in root.GetComponentsInChildren<Collider>(_includeInactive))
                {
                    if (!PassCommonFilters(root, collider))
                        continue;
                    
                    var shape = Convert3D(collider);
                
                    payload.Shapes.Add(shape);
                }
            }
            
            return payload;
        }
        
        private CollisionShapeData Convert3D(Collider collider)
        {
            var transform = collider.transform;
            var baseShapeData = GetBaseShapeData(transform, collider.isTrigger);
            
            var gameObject = collider.gameObject;
            var layer = gameObject.layer;

            switch (collider)
            {
                case BoxCollider box:
                    baseShapeData.Name = collider.gameObject.name;
                    baseShapeData.Type = ECollisionShapeType.Box;
                    baseShapeData.Layer = layer;
                    baseShapeData.LayerName = LayerMask.LayerToName(layer);
                    baseShapeData.Center = transform.TransformPoint(box.center);
                    baseShapeData.Size = Vector3.Scale(box.size, transform.lossyScale);
                    break;
                case SphereCollider sphere:
                    baseShapeData.Name = collider.gameObject.name;
                    baseShapeData.Type = ECollisionShapeType.Sphere;
                    baseShapeData.Layer = layer;
                    baseShapeData.LayerName = LayerMask.LayerToName(layer);
                    baseShapeData.Center = transform.TransformPoint(sphere.center);
                    baseShapeData.Radius = sphere.radius;
                    break;
                case CapsuleCollider capsule:
                    baseShapeData.Name = collider.gameObject.name;
                    baseShapeData.Type = ECollisionShapeType.Capsule;
                    baseShapeData.Layer = layer;
                    baseShapeData.LayerName = LayerMask.LayerToName(layer);
                    baseShapeData.Center = transform.TransformPoint(capsule.center);
                    baseShapeData.Radius = capsule.radius * Mathf.Max(Mathf.Abs(transform.lossyScale.x),
                        Mathf.Abs(transform.lossyScale.y), Mathf.Abs(transform.lossyScale.z));
                    baseShapeData.Height = capsule.height * AxisScale(transform.lossyScale, capsule.direction);
                    baseShapeData.Direction = capsule.direction;
                    break;
                case MeshCollider mesh:
                    baseShapeData.Name = collider.gameObject.name;
                    baseShapeData.Type = ECollisionShapeType.Mesh;
                    baseShapeData.Layer = layer;
                    baseShapeData.LayerName = LayerMask.LayerToName(layer);
                    var bound = mesh.bounds;
                    baseShapeData.Center = bound.center;
                    baseShapeData.Size = bound.size;
                    break;
            }

            return baseShapeData;
        }
        
        private CollisionShapeData GetBaseShapeData(Transform transform, bool isTrigger)
        {
            return new CollisionShapeData
            {
                Type = isTrigger ? ECollisionShapeType.None : ECollisionShapeType.Box,
                Rotation = transform.rotation,
                Scale = transform.localScale,
            };
        }
    }
}
#endif
