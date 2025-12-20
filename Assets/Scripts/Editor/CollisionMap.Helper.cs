#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public partial class CollisionMap
    {
        private static LayerMask LayerMaskField(string label, LayerMask selected)
        {
            var layers = new List<string>();
            var layerNumbers = new List<int>();

            for (var i = 0; i < 32; i++)
            {
                var layerName = LayerMask.LayerToName(i);
                
                if (string.IsNullOrEmpty(layerName)) 
                    continue;
                
                layers.Add(layerName);
                layerNumbers.Add(i);
            }

            var maskWithoutEmpty = 0;
            
            for (var i = 0; i < layerNumbers.Count; i++)
            {
                var layer = layerNumbers[i];
                if (((1 << layer) & selected.value) != 0)
                    maskWithoutEmpty |= 1 << i;
            }

            maskWithoutEmpty = EditorGUILayout.MaskField(label, maskWithoutEmpty, layers.ToArray());

            var mask = layerNumbers.Where((t, i) => (maskWithoutEmpty & (1 << i)) != 0)
                .Aggregate(0, (current, t) => current | 1 << t);

            selected.value = mask;
            return selected;
        }
        
        private bool PassCommonFilters(GameObject gameObject, Collider collider)
        {
            if (!_includeInactive && !gameObject.activeInHierarchy)
                return false;
            
            if (!_includeTriggers && collider.isTrigger)
                return false;
            
            if (!collider.enabled)
                return false;
            
            var bit = 1 << gameObject.layer;
            return (_layerMask.value & bit) != 0;
        }
        
        private static float AxisScale(Vector3 s, int dir) => dir switch
        {
            0 => Mathf.Abs(s.x),
            1 => Mathf.Abs(s.y),
            2 => Mathf.Abs(s.z),
            _ => Mathf.Max(Mathf.Abs(s.x), Mathf.Abs(s.y), Mathf.Abs(s.z))
        };
    }
}
#endif
