using SadJam;
using System;
using System.Globalization;
using UnityEditor;
using UnityEngine;

namespace SadJamEditor
{
    public static class StructComponentGUI<T> where T : struct
    {
        public static void StructComponentField(Rect position, object value, object holder, GameObject context, Type resultType, GUIContent label, Action<object> onDrop = null, params GenericMenuItem[] contextMenuItems)
        {
            StructComponentLabelField(position, label, (StructComponent<T>)value);

            DropperGUI.DragAndDrop(GUILayoutUtility.GetLastRect(), value, holder, context, resultType, onDrop, contextMenuItems);
        }

        public static void StructComponentField(Rect position, object value, object holder, GameObject context, Type resultType, GUIContent label, params GenericMenuItem[] contextMenuItems)
        {
            StructComponentLabelField(position, label, (StructComponent<T>)value);

            DropperGUI.DragAndDrop(GUILayoutUtility.GetLastRect(), value, holder, context, resultType, null, contextMenuItems);
        }

        public static void StructComponentLabelField(Rect pos, GUIContent label, StructComponent<T> component)
        {
            if (component != null)
            {
                EditorGUI.LabelField(pos, GetLabelWithType(label), component.Label + " " + GetSize(component));

                return;
            }

            EditorGUI.LabelField(pos, GetLabelWithType(label), "Drag component on me");
        }

        public static void StructComponentLayoutLabelField(GUIContent label, StructComponent<T> component, params GUILayoutOption[] options)
        {
            if (component != null)
            {
                EditorGUILayout.LabelField(GetLabelWithType(label), component.Label + " " + GetSize(component), options);

                return;
            }

            EditorGUILayout.LabelField(GetLabelWithType(label), "Drag component on me", options);
        }

        public static void StructComponentLayoutLabel(StructComponent<T> component, params GUILayoutOption[] options)
        {
            GUILayout.Label(component.Label + " " + GetSize(component), options);
        }

        private static T GetSize(StructComponent<T> c)
        {
            T size;

            try
            {
                size = c.Size;
            }
            catch
            {
                size = new();
            }

            return size;
        }

        public static void DrawConstantStruct(SerializedObject constantStruct)
        {
            SerializedProperty prop = constantStruct.FindProperty(TypeExtensions.GetBackingFieldName(nameof(StructComponent<T>.Size)));

            EditorGUILayout.PropertyField(prop, new GUIContent(GetLabelWithType(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(nameof(StructComponent<T>.Size)))));
            
        }

        public static object StructField(GUIContent label, T value, params GUILayoutOption[] options)
        {
            return StructField(label.text, value, options);
        }

        public static object StructField(string label, T value, params GUILayoutOption[] options)
        {
            switch (value)
            {
                case Quaternion q:
                    Vector4 v4 = EditorGUILayout.Vector4Field(GetLabelWithType(label), new(q.x, q.y, q.z, q.w), options);
                    return new Quaternion(v4.x, v4.y, v4.z, v4.w);
                case Vector4 v:
                    return EditorGUILayout.Vector4Field(GetLabelWithType(label), v, options);
                case Vector3 v:
                    return EditorGUILayout.Vector3Field(GetLabelWithType(label), v, options);
                case Vector2 v:
                    return EditorGUILayout.Vector2Field(GetLabelWithType(label), v, options);
                case float f:
                    return EditorGUILayout.FloatField(GetLabelWithType(label), f, options);
                case int i:
                    return EditorGUILayout.IntField(GetLabelWithType(label), i, options);
                case double d:
                    return EditorGUILayout.DoubleField(GetLabelWithType(label), d, options);
                case Color c:
                    return EditorGUILayout.ColorField(GetLabelWithType(label), c, options);
            }

            return null;
        }

        public static string GetLabelWithType(GUIContent label) => GetLabelWithType(label.text);
        public static string GetLabelWithType(string label) => label + " (" + typeof(T).Name + ")";
    }
}
