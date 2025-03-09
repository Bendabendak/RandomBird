using SadJam;
using SadJam.Components;
using UnityEngine;

namespace SadJamEditor.Components
{
    public class SplitConvertor_Vector2_X : ConversionType<StructComponent<Vector2>>
    {
        public override ConversionType conversionType => ConversionType.Split;
        public override string uniqueAddress => "X";

        public override object OnSelection(StructComponent<Vector2> input) => Adapter_Float.GetAdapter<Adapter_Float, SizeConvertor_Vector2ToFloat_X>(input.gameObject, new() { input });
    }
}
