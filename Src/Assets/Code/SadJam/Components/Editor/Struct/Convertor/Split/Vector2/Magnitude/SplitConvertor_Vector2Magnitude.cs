using SadJam;
using SadJam.Components;
using UnityEngine;

namespace SadJamEditor.Components
{
    public class SplitConvertor_Vector2Magnitude : ConversionType<StructComponent<Vector2>>
    {
        public override ConversionType conversionType => ConversionType.Split;

        public override string uniqueAddress => "Magnitude";

        public override object OnSelection(StructComponent<Vector2> input) => Adapter_Float.GetAdapter<Adapter_Float, SizeConvertor_Vector2MagnitudeFloat>(input.gameObject, new() { input });
    }
}
