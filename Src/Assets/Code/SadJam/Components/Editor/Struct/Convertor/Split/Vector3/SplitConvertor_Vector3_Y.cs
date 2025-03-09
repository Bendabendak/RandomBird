using SadJam;
using SadJam.Components;
using UnityEngine;

namespace SadJamEditor.Components
{
    public class SplitConvertor_Vector3_Y : ConversionType<StructComponent<Vector3>>
    {
        public override ConversionType conversionType => ConversionType.Split;
        public override string uniqueAddress => "Y";

        public override object OnSelection(StructComponent<Vector3> input) => Adapter_Float.GetAdapter<Adapter_Float, SizeConvertor_Vector3ToFloat_Y>(input.gameObject, new() { input });
    }
}
