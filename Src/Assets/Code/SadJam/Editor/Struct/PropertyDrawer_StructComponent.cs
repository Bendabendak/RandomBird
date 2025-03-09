using UnityEditor;
using UnityEngine;
using SadJam;

namespace SadJamEditor
{
    public abstract class PropertyDrawer_StructComponent<T> : PropertyDrawer where T : struct
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            object value = property.objectReferenceValue;

            UnityEngine.Component target = (UnityEngine.Component)property.serializedObject.targetObject;

            if (property.objectReferenceValue != null)
            {
                Rect prefabRect = new(EditorGUIUtility.labelWidth + 15, position.y, 3, position.height);
                SerializedObject serializedObject = new(property.objectReferenceValue);
                foreach (SerializedProperty p in serializedObject.GetSerializedProperties())
                {
                    if (p.prefabOverride)
                    {
                        EditorGUIExtensions.DrawPrefabOverridden(prefabRect);
                        break;
                    }
                }
            }

            StructComponentGUI<T>.StructComponentField(position, value, target, target.gameObject, typeof(StructComponent<T>), label, (object result) => 
            {
                property.objectReferenceValue = (UnityEngine.Object)result;
                property.serializedObject.ApplyModifiedProperties();
                EditorUtilityExtensions.SetDirty(target, result.GetType().FullName + " dropped on " + target.GetType().FullName);
            });

            EditorGUIExtensions.DrawContextItemsSelector(position, fieldInfo.FieldType, property, label);
            EditorGUI.EndProperty();
        }
    }
}
