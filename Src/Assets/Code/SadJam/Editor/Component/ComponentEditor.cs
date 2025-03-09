using UnityEditor;
using System.Linq;

namespace SadJamEditor
{
    [CustomEditor(typeof(SadJam.Component), true)]
    [CanEditMultipleObjects]
    public class ComponentEditor : Editor
    {
        public void DrawLabel()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_label"));

            EditorGUILayout.Space();
        }

        public void DrawDefaultComponentInspector(params string[] excluding)
        {
            DrawDefaultComponentInspector(true, excluding);
        }

        public void DrawDefaultComponentInspector(bool withLabel, params string[] excluding)
        {
            if (withLabel)
            {
                DrawLabel();
            }

            foreach (SerializedProperty prop in serializedObject.GetVisibleSerializedProperties(false))
            {
                if (excluding.Contains(prop.name)) continue;

                EditorGUIExtensions.DrawProperty(prop);
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawDefaultComponentInspector();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
