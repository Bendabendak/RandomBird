using UnityEditor;
using SadJam.StateMachine;
using SadJam;

namespace SadJamEditor.StateMachine
{
    [CustomEditor(typeof(SetTriggerState))]
    public class SetTriggerState_Editor : DynamicExecutor_Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawDynamicExecutor();

            DrawDefaultComponentInspector(false, TypeExtensions.GetBackingFieldName(nameof(SetTriggerState.States)));

            serializedObject.ApplyModifiedProperties();
        }
    }
}

