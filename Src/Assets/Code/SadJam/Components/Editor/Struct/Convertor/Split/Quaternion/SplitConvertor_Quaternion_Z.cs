using SadJam;
using SadJam.Components;
using UnityEngine;

namespace SadJamEditor.Components
{
    public class SplitConvertor_Quaternion_Z : ConversionType<StructComponent<Quaternion>>
    {
        public override ConversionType conversionType => ConversionType.Split;
        public override string uniqueAddress => "Z";

        public override object OnSelection(StructComponent<Quaternion> input) => Adapter_Float.GetAdapter<Adapter_Float, SizeConvertor_QuaternionToFloat_Z>(input.gameObject, new() { input });
    }
}
