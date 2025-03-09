using SadJam;
using SadJam.Components;
using UnityEngine;

namespace SadJamEditor.Components
{
    public class SplitConvertor_Quaternion_X : ConversionType<StructComponent<Quaternion>>
    {
        public override ConversionType conversionType => ConversionType.Split;
        public override string uniqueAddress => "X";

        public override object OnSelection(StructComponent<Quaternion> input) => Adapter_Float.GetAdapter<Adapter_Float, SizeConvertor_QuaternionToFloat_X>(input.gameObject, new() { input });
    }
}