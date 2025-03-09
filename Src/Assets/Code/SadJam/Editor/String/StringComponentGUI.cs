using System;
using SadJam;
using UnityEngine;
using UnityEditor;

namespace SadJamEditor
{
    public static class StringComponentGUI
    {
        public static void StringComponentField(Rect position, object value, object holder, GameObject context, Type resultType, GUIContent label, Action<object> onDrop = null, params GenericMenuItem[] contextMenuItems)
        {
            StringComponentLabelField(position, label, (StringComponent)value);

            DropperGUI.DragAndDrop(GUILayoutUtility.GetLastRect(), value, holder, context, resultType, onDrop, contextMenuItems);
        }

        public static void StringComponentLabelField(Rect pos, GUIContent label, StringComponent component)
        {
            if (component != null)
            {
                EditorGUI.LabelField(pos, GetLabelWithType(label), component.Label + " " + component.Content);

                return;
            }

            EditorGUI.LabelField(pos, GetLabelWithType(label), "Drag component on me");
        }

        public static void StringComponentLayoutLabelField(GUIContent label, StringComponent component, params GUILayoutOption[] options)
        {
            if (component != null)
            {
                EditorGUILayout.LabelField(GetLabelWithType(label), component.Label + " " + component.Content, options);

                return;
            }

            EditorGUILayout.LabelField(GetLabelWithType(label), "Drag component on me", options);
        }

        public static void StringComponentLayoutLabel(StringComponent component, params GUILayoutOption[] options)
        {
            GUILayout.Label(component.Label + " " + component.Content, options);
        }

        public static string GetLabelWithType(GUIContent label) => label.text + " (" + typeof(string).Name + ")";
        public static string GetLabelWithType(string label) => label + " (" + typeof(string).Name + ")";
    }
}
