#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Utils.SerializedDictionary;

namespace Editor
{
    [CustomPropertyDrawer(typeof(UnitySerializedDictionaryBase<,>), true)]
    public class UnitySerializedDictionaryDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            var keys = property.FindPropertyRelative("_keyData");
            var values = property.FindPropertyRelative("_valueData");

            if (keys == null || values == null)
            {
                EditorGUI.LabelField(position, "Dictionary fields not found");
                EditorGUI.EndProperty();
                return;
            }
            
            EditorGUILayout.LabelField(label.text, EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Key", EditorStyles.miniBoldLabel);
            GUILayout.Label("Value", EditorStyles.miniBoldLabel);
            GUILayout.Space(22);
            EditorGUILayout.EndHorizontal();

            var count = Mathf.Max(keys.arraySize, values.arraySize);
            
            for (var i = 0; i < count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                if (i >= keys.arraySize) 
                    keys.InsertArrayElementAtIndex(i);
                
                if (i >= values.arraySize) 
                    values.InsertArrayElementAtIndex(i);

                EditorGUILayout.PropertyField(keys.GetArrayElementAtIndex(i), GUIContent.none, GUILayout.MinWidth(80));
                EditorGUILayout.PropertyField(values.GetArrayElementAtIndex(i), GUIContent.none, GUILayout.MinWidth(80));

                if (GUILayout.Button("–", GUILayout.Width(20)))
                {
                    keys.DeleteArrayElementAtIndex(i);
                    values.DeleteArrayElementAtIndex(i);
                    break;
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space(4);
            
            if (GUILayout.Button("Add Entry"))
            {
                var newIndex = keys.arraySize;
                
                keys.arraySize++;
                values.arraySize++;
                
                var newKey = keys.GetArrayElementAtIndex(newIndex);
                var newValue = values.GetArrayElementAtIndex(newIndex);
                
                ResetPropertyValue(newKey);
                ResetPropertyValue(newValue);
                
                property.serializedObject.ApplyModifiedProperties();
                GUI.FocusControl(null);
            }

            EditorGUI.EndProperty();
        }

        private void ResetPropertyValue(SerializedProperty property)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    property.intValue = 0;
                    break;
                case SerializedPropertyType.Boolean:
                    property.boolValue = false;
                    break;
                case SerializedPropertyType.Float:
                    property.floatValue = 0f;
                    break;
                case SerializedPropertyType.String:
                    property.stringValue = "";
                    break;
                case SerializedPropertyType.Color:
                    property.colorValue = Color.white;
                    break;
                case SerializedPropertyType.ObjectReference:
                    property.objectReferenceValue = null;
                    break;
                case SerializedPropertyType.Enum:
                    property.enumValueIndex = 0;
                    break;
                case SerializedPropertyType.Vector2:
                    property.vector2Value = Vector2.zero;
                    break;
                case SerializedPropertyType.Vector3:
                    property.vector3Value = Vector3.zero;
                    break;
                case SerializedPropertyType.Vector4:
                    property.vector4Value = Vector4.zero;
                    break;
                case SerializedPropertyType.Rect:
                    property.rectValue = new Rect();
                    break;
                case SerializedPropertyType.AnimationCurve:
                    property.animationCurveValue = AnimationCurve.Linear(0, 0, 1, 1);
                    break;
                case SerializedPropertyType.Bounds:
                    property.boundsValue = new Bounds();
                    break;
                case SerializedPropertyType.Quaternion:
                    property.quaternionValue = Quaternion.identity;
                    break;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 0f;
        }
    }
}
#endif
