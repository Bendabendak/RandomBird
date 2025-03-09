using SadJam;
using UnityEditor;
using UnityEngine;

namespace SadJamEditor
{
    [CustomPropertyDrawer(typeof(Abs), true)]
    public class Abs_PropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Float:
                    property.floatValue = Mathf.Abs(property.floatValue);
                    break;

                case SerializedPropertyType.Integer:
                    property.intValue = Mathf.Abs(property.intValue);
                    break;

                case SerializedPropertyType.Vector2:
                    property.vector2Value = new(Mathf.Abs(property.vector2Value.x), Mathf.Abs(property.vector2Value.y));
                    break;

                case SerializedPropertyType.Vector2Int:
                    property.vector2IntValue = new(Mathf.Abs(property.vector2IntValue.x), Mathf.Abs(property.vector2IntValue.y));
                    break;


                case SerializedPropertyType.Vector3:
                    property.vector3Value = new(Mathf.Abs(property.vector3Value.x), Mathf.Abs(property.vector3Value.y), Mathf.Abs(property.vector3Value.z));
                    break;

                case SerializedPropertyType.Vector3Int:
                    property.vector3IntValue = new(Mathf.Abs(property.vector3IntValue.x), Mathf.Abs(property.vector3IntValue.y), Mathf.Abs(property.vector3IntValue.z));
                    break;

                case SerializedPropertyType.Vector4:
                    property.vector4Value = new(Mathf.Abs(property.vector4Value.x), Mathf.Abs(property.vector4Value.y), Mathf.Abs(property.vector4Value.z), Mathf.Abs(property.vector4Value.w));
                    break;
            }

            EditorGUI.PropertyField(position, property, label);

            property.serializedObject.ApplyModifiedProperties();
        }
    }
}
