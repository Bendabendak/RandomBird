using SadJam;
using UnityEditor;
using UnityEngine;

namespace SadJamEditor
{
    [CustomPropertyDrawer(typeof(GameConfig_Selection<>), true)]
    public class GameConfigSelection_PropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            SerializedProperty configProp = property.FindPropertyRelative("_config");
            SerializedProperty configSelectorProp = property.FindPropertyRelative("_configSelector");

            float height;

            if(configProp.objectReferenceValue != null)
            {
                UnityEngine.Object selected = EditorGUI.ObjectField(position, label, configProp.objectReferenceValue, typeof(GameConfig), true);
                
                if (configProp.objectReferenceValue != selected)
                {
                    configProp.objectReferenceValue = selected;
                    configSelectorProp.objectReferenceValue = null;
                }

                height = EditorGUI.GetPropertyHeight(configProp);
            }
            else if(configSelectorProp.objectReferenceValue != null)
            {
                UnityEngine.Object selected = EditorGUI.ObjectField(position, label, configSelectorProp.objectReferenceValue, typeof(GameConfig_Selector), true);

                if (configSelectorProp.objectReferenceValue != selected)
                {
                    configProp.objectReferenceValue = null;
                    configSelectorProp.objectReferenceValue = selected;
                }

                height = EditorGUI.GetPropertyHeight(configSelectorProp);
            }
            else
            {
                height = EditorGUIExtensions.Foldout(position, label, property, configProp, configSelectorProp);
            }

            EditorGUIExtensions.SetPropertyHeight(property, height);
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => EditorGUIExtensions.GetPropertyHeight(property);
    }
}