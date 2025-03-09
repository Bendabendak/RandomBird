using SadJam;
using UnityEngine;

namespace SadJamEditor
{
    public abstract class Editor_StructCalculatorComponent<T> : ComponentEditor where T : struct
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawLabel();
            serializedObject.ApplyModifiedProperties();

            StructCalculatorComponent<T> component = (StructCalculatorComponent<T>)target;

            EditorGUIExtensions.Result(component.Size);

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Edit"))
            {
                OnEdit();
            }
            GUILayout.FlexibleSpace();
        }

        protected abstract void OnEdit();
    }
}
