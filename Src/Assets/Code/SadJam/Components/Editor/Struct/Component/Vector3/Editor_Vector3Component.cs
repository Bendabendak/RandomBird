using SadJam;
using UnityEditor;
using UnityEngine;

namespace SadJamEditor.Components
{
    [CustomEditor(typeof(StructComponent<Vector3>), true)]
    [CanEditMultipleObjects]
    public class Editor_Vector3Component : Editor_StructComponent<Vector3>
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

            Vector3 val;
            try
            {
                val = _getter();
            }
            catch
            {
                EditorGUILayout.Vector3Field("Size", Vector3.zero);
                GUI.enabled = true;

                return;
            }

            EditorGUILayout.Vector3Field("Size", val);
            GUI.enabled = true;
        }
    }
}
