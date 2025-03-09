using System.Collections.Generic;
using UnityEngine;

namespace SadJam.Components
{
    public class SizeConvertor_Vector2NormalizedToFloat_Y : SizeConvertor<float>
    {
        public override Label ConversionLabel => new("Normalized");

        public override float ConvertSize(List<UnityEngine.Component> inputs, string[] customData)
            => ((StructComponent<Vector2>)inputs[0]).Size.normalized.y;
    }
}
