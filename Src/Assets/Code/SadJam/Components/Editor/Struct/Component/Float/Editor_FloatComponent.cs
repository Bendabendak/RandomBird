using SadJam;
using UnityEditor;
using UnityEngine;

namespace SadJamEditor.Components
{
    [CustomEditor(typeof(StructComponent<float>), true)]
    [CanEditMultipleObjects]
    public class Editor_FloatComponent : Editor_StructComponent<float>
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawLabel();
            DrawSize();
            DrawDefaultComponentInspector(false);

            serializedObject.ApplyModifiedProperties();
        }
        public void DrawSize()
        {
            GUI.enabled = false;

            float val;
            try
            {
                val = _getter();
            }
            catch
            {
                EditorGUILayout.FloatField("Size", 0);
                GUI.enabled = true;

                return;
            }

            EditorGUILayout.FloatField("Size", val);
            GUI.enabled = true;
        }
    }
}
