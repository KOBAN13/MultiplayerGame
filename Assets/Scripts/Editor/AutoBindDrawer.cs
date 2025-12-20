#if UNITY_EDITOR
using System;
using System.Reflection;
using UI.Base;
using UI.Helpers;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Editor
{
    public static class AutoBindGuidEnsurer
    {
        private static BindingFlags GuidFlags => BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty;
        
        [MenuItem("Tools/AutoBind/Ensure GUIDs In Project")]
        public static void EnsureGuidsInProject()
        {
            int processed = 0, updated = 0;
        
            var guids = AssetDatabase.FindAssets("t:Object");
            
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (string.IsNullOrEmpty(path))
                    continue;

                var asset = AssetDatabase.LoadAssetAtPath<Object>(path);
                
                if (asset == null)
                    continue;

                if (TryEnsureGuids(asset))
                    updated++;

                processed++;
            }

            Debug.Log($"[AutoBind] Проверено {processed} ассетов, обновлено {updated} GUIDов.");
        }
        
        [MenuItem("Tools/AutoBind/Clear GUIDs In Project")]
        public static void ClearGuidsInProject()
        {
            var guids = AssetDatabase.FindAssets("t:Object");
            
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (string.IsNullOrEmpty(path))
                    continue;

                var asset = AssetDatabase.LoadAssetAtPath<Object>(path);
                
                if (asset == null)
                    continue;
                
                ClearGuids(asset);
            }


            Debug.Log("[AutoBind] Все GUID успешно очищены.");
        }
        
        private static void ClearGuids(Object asset)
        {
            var type = asset.GetType();
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var field in fields)
            {
                if (!Attribute.IsDefined(field, typeof(AutoBindAttribute)))
                    continue;

                var value = field.GetValue(asset);
                if (value == null)
                    continue;

                var guidField = value.GetType().GetProperty("AutoBindId", GuidFlags);

                if (guidField == null)
                    continue;

                var autoBindId = guidField.GetValue(value) as AutoBindId;
                if (autoBindId == null)
                    continue;
                
                autoBindId.ClearGuid();
                
                EditorUtility.SetDirty(asset);
                AssetDatabase.SaveAssetIfDirty(asset);
            }
            
            if (asset is GameObject go)
            {
                foreach (var mb in go.GetComponentsInChildren<MonoBehaviour>(true))
                {
                    if (mb == null) 
                        continue;
                    
                    ClearGuids(mb);
                }
            }
        }
        
        private static bool TryEnsureGuids(Object asset)
        {
            var modified = false;
        
            var type = asset.GetType();
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var field in fields)
            {
                if (!Attribute.IsDefined(field, typeof(AutoBindAttribute)))
                    continue;

                var value = (ViewBinder) field.GetValue(asset);
                
                if (value == null)
                    continue;

                var guidField = value.GetType().GetProperty("AutoBindId", GuidFlags);

                if (guidField == null)
                    continue;

                var autoBindId = guidField.GetValue(value) as AutoBindId;
                if (autoBindId == null)
                    continue;

                var before = autoBindId.GeneratedGuid;
                autoBindId.EnsureGuid();
                
                if (autoBindId.GeneratedGuid != before)
                {
                    var guid = value.AutoBindId.GeneratedGuid;
                    value.ViewModelBinder.AutoBindId.SetGuid(guid);
                    
                    modified = true;
                    EditorUtility.SetDirty(asset);
                    
                    Debug.Log($"[AutoBind] Генерирован GUID для {asset.name} ({type.Name}.{field.Name})");
                }
            }
            
            if (asset is GameObject go)
            {
                foreach (var mb in go.GetComponentsInChildren<MonoBehaviour>(true))
                {
                    if (mb == null) 
                        continue;
                    
                    if (TryEnsureGuids(mb))
                        modified = true;
                }
            }

            if (modified)
            {
                AssetDatabase.SaveAssetIfDirty(asset);
            }

            return modified;
        }
    }
}
#endif
