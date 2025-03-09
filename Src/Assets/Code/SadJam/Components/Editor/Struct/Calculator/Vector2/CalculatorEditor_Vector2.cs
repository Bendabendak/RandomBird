using UnityEngine;
using UnityEditor;
using SadJam;
using SadJam.Components;

namespace SadJamEditor.Components
{
    [CustomEditor(typeof(Calculator_Vector2), true)]
    [CanEditMultipleObjects]
    public class CalculatorEditor_Vector2 : Editor_StructCalculatorComponent<Vector2>
    {
        protected override void OnEdit()
        {
            CreateInstance<CalculatorEditorWindow_Vector2>().Open(target as StructCalculatorComponent<Vector2>);
        }
    }
}