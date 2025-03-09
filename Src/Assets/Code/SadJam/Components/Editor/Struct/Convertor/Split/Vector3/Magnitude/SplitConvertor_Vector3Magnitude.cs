using SadJam;
using SadJam.Components;
using UnityEngine;

namespace SadJamEditor.Components
{
    public class SplitConvertor_Vector3Magnitude : ConversionType<StructComponent<Vector3>>
    {
        public override ConversionType conversionType => ConversionType.Split;

        public override string uniqueAddress => "Magnitude";

        public override object OnSelection(StructComponent<Vector3> input) => Adapter_Float.GetAdapter<Adapter_Float, SizeConvertor_Vector3MagnitudeFloat>(input.gameObject, new() { input });
    }
}
