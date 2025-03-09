using SadJam;
using UnityEditor;
using UnityEngine;

namespace SadJamEditor.Components
{
    [CustomEditor(typeof(StructComponent<Quaternion>), true)]
    [CanEditMultipleObjects]
    public class Editor_QuaternionComponent : Editor_StructComponent<Quaternion>
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

            Quaternion val;
            try
            {
                val = _getter();
            }
            catch
            {
                EditorGUILayout.Vector4Field("Size", Vector4.zero);
                GUI.enabled = true;

                return;
            }

            Vector4 v = new(val.x, val.y, val.z, val.w);
            EditorGUILayout.Vector4Field("Size", v);
            GUI.enabled = true;
        }
    }
}
