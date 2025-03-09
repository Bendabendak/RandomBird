using SadJam;
using UnityEngine;

namespace SadJamEditor.Components
{
    public class CalculatorEditor_Quaternion : Editor_StructCalculatorComponent<Quaternion>
    {
        protected override void OnEdit()
        {
            CreateInstance<CalculatorEditorWindow_Quaternion>().Open(target as StructCalculatorComponent<Quaternion>);
        }
    }
}
