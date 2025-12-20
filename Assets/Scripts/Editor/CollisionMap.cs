#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Editor
{
    public partial class CollisionMap : EditorWindow
    {
        [SerializeField] private SceneAsset _scene;
        
        [Header("Filters")]
        [SerializeField] private bool _includeTriggers;
        [SerializeField] private bool _includeInactive;
        [SerializeField] private LayerMask _layerMask = ~0;

        [MenuItem("Tools/Collect Collision Map")]
        private static void OpenWindow()
        {
            var window = GetWindow<CollisionMap>();
            window.titleContent = new GUIContent("Collision Map");
            window.minSize = new Vector2(400, 300);
            window.Show();
        }
        
        private void OnGUI()
        {
            EditorGUILayout.LabelField("Export collisions from a scene by name", EditorStyles.boldLabel);

            using (new EditorGUILayout.VerticalScope("box"))
            {
                EditorGUILayout.LabelField("Scene", EditorStyles.boldLabel);

                _scene = (SceneAsset)EditorGUILayout.ObjectField(
                    "Scene Asset", _scene, typeof(SceneAsset), allowSceneObjects: false);
            }

            using (new EditorGUILayout.VerticalScope("box"))
            {
                EditorGUILayout.LabelField("Filters", EditorStyles.boldLabel);
                _includeTriggers = EditorGUILayout.Toggle("Include Triggers", _includeTriggers);
                _includeInactive = EditorGUILayout.Toggle("Include Inactive", _includeInactive);
                _layerMask = LayerMaskField("Layer Mask", _layerMask);
            }

            GUILayout.FlexibleSpace();

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Collect & Export", GUILayout.Height(32)))
                {
                    try
                    {
                        var scenePath = GetScenePath();
                        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);

                        var payload = CollectFromScene(scene);
                        UploadToServer(payload);
                        EditorSceneManager.CloseScene(scene, true);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                        EditorUtility.DisplayDialog("Collision Exporter", $"Error:\n{e.Message}", "OK");
                    }
                }
            }
        }
        
        private string GetScenePath()
        {
            string scenePath = null;

            if (_scene == null) 
                return null;
            
            scenePath = AssetDatabase.GetAssetPath(_scene);
                
            if (string.IsNullOrEmpty(scenePath) || !scenePath.EndsWith(".unity", StringComparison.OrdinalIgnoreCase))
                throw new Exception("Selected asset is not a .unity scene.");

            return scenePath;
        }
    }
}
#endif
