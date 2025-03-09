using SadJam;
using SadJam.Components;
using UnityEngine;

namespace SadJamEditor.Components
{
    public class NoramlConvertor_Vector3Normalized : ConversionType<StructComponent<Vector3>>
    {
        public override ConversionType conversionType => ConversionType.Normal;

        public override string uniqueAddress => "Normalized";

        public override object OnSelection(StructComponent<Vector3> input)
            => Adapter_Vector3.GetAdapter<Adapter_Vector3, SizeConvertor_Vector3Normalized>(input.gameObject, new() { input });
    }
}

