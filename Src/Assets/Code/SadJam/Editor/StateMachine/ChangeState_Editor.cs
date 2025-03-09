using UnityEditor;
using SadJam.StateMachine;

namespace SadJamEditor.StateMachine
{
    [CustomEditor(typeof(ChangeState))]
    public class ChangeState_Editor : DynamicExecutor_Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawDynamicExecutor();

            DrawDefaultComponentInspector(false, nameof(ChangeState.Triggers));

            serializedObject.ApplyModifiedProperties();
        }
    }
}
