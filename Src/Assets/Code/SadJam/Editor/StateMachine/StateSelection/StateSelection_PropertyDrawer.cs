using UnityEngine;
using UnityEditor;
using SadJam.StateMachine;

namespace SadJamEditor.StateMachine
{
    [CustomPropertyDrawer(typeof(Selection_State))]
    public class StateSelection_PropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty globalStateChangerProp = property.FindPropertyRelative(nameof(Selection_State.GlobalStateHolder));
            SerializedProperty stateProp = property.FindPropertyRelative(nameof(Selection_State.State));
            SerializedProperty isLocalProp = property.FindPropertyRelative(nameof(Selection_State.Local));
            SerializedProperty categoryProp = property.FindPropertyRelative(nameof(Selection_State.Category));

            bool isLocal = isLocalProp.boolValue;

            float height;
            if (!isLocal)
            {
                height = EditorGUIExtensions.Foldout(position, label, property, isLocalProp, globalStateChangerProp, categoryProp, stateProp);
            }
            else
            {
                height = EditorGUIExtensions.Foldout(position, label, property, isLocalProp, categoryProp, stateProp);
            }

            EditorGUIExtensions.SetPropertyHeight(property, height);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => EditorGUIExtensions.GetPropertyHeight(property);
    }
}
