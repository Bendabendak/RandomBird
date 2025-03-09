using System.Collections.Generic;
using UnityEngine;

namespace SadJam.Components
{
    public class SizeConvertor_Vector3NormalizedToFloat_X : SizeConvertor<float>
    {
        public override Label ConversionLabel => new("Normalized");

        public override float ConvertSize(List<UnityEngine.Component> inputs, string[] customData)
            => ((StructComponent<Vector3>)inputs[0]).Size.normalized.x;
    }
}
