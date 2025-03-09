using System.Collections.Generic;
using UnityEngine;

namespace SadJam.Components
{
    public class SizeConvertor_Vector3Normalized : SizeConvertor<Vector3>
    {
        public override Label ConversionLabel => new("Normalized");

        public override Vector3 ConvertSize(List<UnityEngine.Component> inputs, string[] customData)
            => ((StructComponent<Vector3>)inputs[0]).Size.normalized;
    }
}
