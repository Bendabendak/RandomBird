using UnityEngine;
using UnityEditor;
using SadJam.Components;
using SadJam;

namespace SadJamEditor.Components
{
    [CustomEditor(typeof(Calculator_Vector3), true)]
    [CanEditMultipleObjects]
    public class CalculatorEditor_Vector3 : Editor_StructCalculatorComponent<Vector3>
    {
        protected override void OnEdit()
        {
            CreateInstance<CalculatorEditorWindow_Vector3>().Open(target as StructCalculatorComponent<Vector3>);
        }
    }
}