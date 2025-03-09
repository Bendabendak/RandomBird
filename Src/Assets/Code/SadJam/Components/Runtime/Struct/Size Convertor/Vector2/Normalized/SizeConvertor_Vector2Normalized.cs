using System.Collections.Generic;
using UnityEngine;

namespace SadJam.Components
{
    public class SizeConvertor_Vector2Normalized : SizeConvertor<Vector2>
    {
        public override Label ConversionLabel => new("Normalized");

        public override Vector2 ConvertSize(List<UnityEngine.Component> inputs, string[] customData)
            => ((StructComponent<Vector2>)inputs[0]).Size.normalized;
    }
}
