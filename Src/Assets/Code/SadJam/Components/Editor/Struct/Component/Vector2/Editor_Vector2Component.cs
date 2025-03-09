using SadJam;
using UnityEditor;
using UnityEngine;

namespace SadJamEditor.Components
{
    [CustomEditor(typeof(StructComponent<Vector2>), true)]
    [CanEditMultipleObjects]
    public class Editor_Vector2Component : Editor_StructComponent<Vector2>
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

            Vector2 val;
            try
            {
                val = _getter();
            }
            catch
            {
                EditorGUILayout.Vector2Field("Size", Vector2.zero);
                GUI.enabled = true;

                return;
            }

            EditorGUILayout.Vector2Field("Size", val);
            GUI.enabled = true;
        }
    }
}

