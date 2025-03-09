using UnityEngine;
using UnityEditor;
using SadJam;

namespace SadJamEditor
{
    [CustomPropertyDrawer(typeof(StringComponent), true)]
    public class PropertyDrawer_StringComponent : PropertyDrawer
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
                foreach(SerializedProperty p in serializedObject.GetSerializedProperties())
                {
                    if (p.prefabOverride)
                    {
                        EditorGUIExtensions.DrawPrefabOverridden(prefabRect);
                        break;
                    }
                }
            }

            StringComponentGUI.StringComponentField(position, value, target, target.gameObject, fieldInfo.FieldType, label, (object result)=> 
            {
                property.objectReferenceValue = (UnityEngine.Component)result;
                property.serializedObject.ApplyModifiedProperties();
                EditorUtilityExtensions.SetDirty(target, result.GetType().FullName + " dropped on " + target.GetType().FullName);
            });

            EditorGUIExtensions.DrawContextItemsSelector(position, fieldInfo.FieldType, property, label);
            EditorGUI.EndProperty();
        }
    }
}
