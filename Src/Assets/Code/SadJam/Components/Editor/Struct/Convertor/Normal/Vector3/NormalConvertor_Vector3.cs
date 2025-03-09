using SadJam;
using UnityEngine;

namespace SadJamEditor.Components
{
    public class NormalConvertor_Vector3 : ConversionType<StructComponent<Vector3>>
    {
        public override ConversionType conversionType => ConversionType.Normal;

        public override string uniqueAddress => "Normal";

        public override object OnSelection(StructComponent<Vector3> input) => input;
    }
}
