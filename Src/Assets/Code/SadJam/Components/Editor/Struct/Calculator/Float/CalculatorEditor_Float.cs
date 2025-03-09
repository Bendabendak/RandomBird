using System;
using UnityEditor;
using SadJam.Components;
using SadJam;

namespace SadJamEditor.Components
{
    [CustomEditor(typeof(Calculator_Float), true)]
    [CanEditMultipleObjects]
    public class CalculatorEditor_Float : Editor_StructCalculatorComponent<Single>
    {
        protected override void OnEdit()
        {
            CreateInstance<CalculatorEditorWindow_Float>().Open(target as StructCalculatorComponent<float>);
        }
    }
}