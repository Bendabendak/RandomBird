using SadJam;
using SadJam.Components;
using UnityEngine;

namespace SadJamEditor.Components
{
    public class NoramlConvertor_Vector2Normalized : ConversionType<StructComponent<Vector2>>
    {
        public override ConversionType conversionType => ConversionType.Normal;

        public override string uniqueAddress => "Normalized";

        public override object OnSelection(StructComponent<Vector2> input) => Adapter_Vector2.GetAdapter<Adapter_Vector2, SizeConvertor_Vector2Normalized>(input.gameObject, new() { input });
    }
}
