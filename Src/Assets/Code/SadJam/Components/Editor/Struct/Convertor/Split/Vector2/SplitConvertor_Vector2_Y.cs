using SadJam;
using SadJam.Components;
using UnityEngine;

namespace SadJamEditor.Components
{
    public class SplitConvertor_Vector2_Y : ConversionType<StructComponent<Vector2>>
    {
        public override ConversionType conversionType => ConversionType.Split;

        public override string uniqueAddress => "Y";

        public override object OnSelection(StructComponent<Vector2> input) => Adapter_Float.GetAdapter<Adapter_Float, SizeConvertor_Vector2ToFloat_Y>(input.gameObject, new() { input });
    }
}
