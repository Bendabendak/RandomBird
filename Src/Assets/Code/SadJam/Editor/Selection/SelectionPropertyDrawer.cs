using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using SadJam;
using System.Linq;

namespace SadJamEditor
{
    [CustomPropertyDrawer(typeof(SadJam.Selection))]
    public class SelectionPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty selection = property.FindPropertyRelative(TypeExtensions.GetBackingFieldName(nameof(SadJam.Selection.Collection)));
            SerializedProperty selected = property.FindPropertyRelative(TypeExtensions.GetBackingFieldName(nameof(SadJam.Selection.Selected)));

            List<string> list = selection.GetElements().Select(s => s.stringValue).ToList();

            int s = EditorGUI.Popup(position, label.text, list.IndexOf(selected.stringValue), list.ToArray());

            if (s >= 0)
            {
                if (list[s] != selected.stringValue)
                {
                    selected.stringValue = list[s];
                }
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => EditorGUIUtility.singleLineHeight;
    }
}
