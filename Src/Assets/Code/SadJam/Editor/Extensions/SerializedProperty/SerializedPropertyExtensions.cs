using SadJam;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace SadJamEditor
{
    public static class SerializedPropertyExtensions
    {

        public static FieldInfo GetField(this SerializedProperty prop) => TypeExtensions.GetFieldViaPath(prop.serializedObject.targetObject.GetType(), prop.propertyPath);

        public static IEnumerable<SerializedProperty> GetElements(this SerializedProperty property)
        {
            if (property.isArray)
            {
                SerializedProperty p = property.Copy();

                p.Next(true);
                p.Next(true);

                int arrayLength = p.intValue;

                p.Next(true);

                int lastIndex = arrayLength - 1;
                for (int i = 0; i < arrayLength; i++)
                {
                    yield return p.Copy();
                    if (i < lastIndex) p.Next(false);
                }
            }
        }
        public static object GetValue(this SerializedProperty property)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    return property.intValue;
                case SerializedPropertyType.Boolean:
                    return property.boolValue;
                case SerializedPropertyType.Float:
                    return property.floatValue;
                case SerializedPropertyType.String:
                    return property.stringValue;
                case SerializedPropertyType.Color:
                    return property.colorValue;
                case SerializedPropertyType.ObjectReference:
                    return property.objectReferenceValue;
                case SerializedPropertyType.AnimationCurve:
                    return property.animationCurveValue;
                case SerializedPropertyType.Bounds:
                    return property.boundsValue;
                case SerializedPropertyType.BoundsInt:
                    return property.boundsIntValue;
                case SerializedPropertyType.Enum:
                    return property.enumValueFlag;
                case SerializedPropertyType.Vector2:
                    return property.vector2Value;
                case SerializedPropertyType.Vector2Int:
                    return property.vector2IntValue;
                case SerializedPropertyType.Vector3:
                    return property.vector3Value;
                case SerializedPropertyType.Vector3Int:
                    return property.vector3IntValue;
                case SerializedPropertyType.Vector4:
                    return property.vector4Value;
                case SerializedPropertyType.Quaternion:
                    return property.quaternionValue;
                case SerializedPropertyType.Rect:
                    return property.rectValue;
                case SerializedPropertyType.RectInt:
                    return property.rectIntValue;
                case SerializedPropertyType.ManagedReference:
                    return property.managedReferenceValue;
                case SerializedPropertyType.ExposedReference:
                    return property.exposedReferenceValue;
                case SerializedPropertyType.Hash128:
                    return property.hash128Value;
            }

            return null;
        }

        public static void SetValue(this SerializedProperty property, object value)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    property.intValue = (int)value;
                    return;
                case SerializedPropertyType.Boolean:
                    property.boolValue = (bool)value;
                    return;
                case SerializedPropertyType.Float:
                    property.floatValue = (float)value;
                    return;
                case SerializedPropertyType.String:
                    property.stringValue = (string)value;
                    return;
                case SerializedPropertyType.Color:
                    property.colorValue = (Color)value;
                    return;
                case SerializedPropertyType.ObjectReference:
                    property.objectReferenceValue = (UnityEngine.Object)value;
                    return;
                case SerializedPropertyType.AnimationCurve:
                    property.animationCurveValue = (AnimationCurve)value;
                    return;
                case SerializedPropertyType.Bounds:
                    property.boundsValue = (Bounds)value;
                    return;
                case SerializedPropertyType.BoundsInt:
                    property.boundsIntValue = (BoundsInt)value;
                    return;
                case SerializedPropertyType.Enum:
                    property.enumValueFlag = (int)value;
                    return;
                case SerializedPropertyType.Vector2:
                    property.vector2Value = (Vector2)value;
                    return;
                case SerializedPropertyType.Vector2Int:
                    property.vector2IntValue = (Vector2Int)value;
                    return;
                case SerializedPropertyType.Vector3:
                    property.vector3Value = (Vector3)value;
                    return;
                case SerializedPropertyType.Vector3Int:
                    property.vector3IntValue = (Vector3Int)value;
                    return;
                case SerializedPropertyType.Vector4:
                    property.vector4Value = (Vector4)value;
                    return;
                case SerializedPropertyType.Quaternion:
                    property.quaternionValue = (Quaternion)value;
                    return;
                case SerializedPropertyType.Rect:
                    property.rectValue = (Rect)value;
                    return;
                case SerializedPropertyType.RectInt:
                    property.rectIntValue = (RectInt)value;
                    return;
                case SerializedPropertyType.ManagedReference:
                    property.managedReferenceValue = value;
                    return;
                case SerializedPropertyType.ExposedReference:
                    property.exposedReferenceValue = (UnityEngine.Object)value;
                    return;
                case SerializedPropertyType.Hash128:
                    property.hash128Value = (Hash128)value;
                    return;
            }
        }
    }
}
