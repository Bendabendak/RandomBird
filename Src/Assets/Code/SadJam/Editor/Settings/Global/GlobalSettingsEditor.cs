using UnityEditor;
using SadJam;
using UnityEngine;

namespace SadJamEditor
{
    public class GlobalSettingsEditor : EditorWindow
    {
        [MenuItem("SadJam/Settings/Global")]
        private static void ShowFromMenu()
        {
            GlobalSettings.Load();
            GetWindow<GlobalSettingsEditor>().Show();
        }

        protected virtual void OnGUI()
        {
            float labelWidth = EditorGUIUtility.labelWidth;
            float maxWidth = position.width / 4;
            foreach (GlobalSettingsMember m in GlobalSettings.Members)
            {
                if (m == null) continue;

                EditorGUIUtility.labelWidth = EditorStyles.label.CalcSize(new GUIContent(m.Label)).x;
                switch (m.Type)
                {
                    case SettingsMemberType.Bool:
                        m.Value = EditorGUILayout.Toggle(m.Label, (bool)m.Value, GUILayout.MaxWidth(maxWidth + EditorGUIUtility.labelWidth));
                        break;
                    case SettingsMemberType.String:
                        m.Value = EditorGUILayout.TextField(m.Label, (string)m.Value, GUILayout.MaxWidth(maxWidth + EditorGUIUtility.labelWidth));
                        break;
                    case SettingsMemberType.Float:
                        m.Value = EditorGUILayout.FloatField(m.Label, (float)m.Value, GUILayout.MaxWidth(maxWidth + EditorGUIUtility.labelWidth));
                        break;
                    case SettingsMemberType.Int:
                        m.Value = EditorGUILayout.IntField(m.Label, (int)m.Value, GUILayout.MaxWidth(maxWidth + EditorGUIUtility.labelWidth));
                        break;
                }
            }

            EditorGUIUtility.labelWidth = labelWidth;

            if (GUILayout.Button("Save"))
            {
                Save();
            }
        }

        public static void Save()
        {
            foreach (GlobalSettingsMember m in GlobalSettings.Members)
            {
                if (m == null) continue;

                switch (m.Type)
                {
                    case SettingsMemberType.Bool:
                        EditorPrefs.SetBool(m.name, (bool)m.Value);
                        break;
                    case SettingsMemberType.String:
                        EditorPrefs.SetString(m.name, (string)m.Value);
                        break;
                    case SettingsMemberType.Float:
                        EditorPrefs.SetFloat(m.name, (float)m.Value);
                        break;
                    case SettingsMemberType.Int:
                        EditorPrefs.SetInt(m.name, (int)m.Value);
                        break;
                }
            }

            GlobalSettings.OnSave?.Invoke();
        }
    }
}
