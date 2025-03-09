using SadJam;
using UnityEngine;

namespace SadJamEditor.Components
{
    public class NormalConvertor_Vector2 : ConversionType<StructComponent<Vector2>>
    {
        public override string uniqueAddress => "Normal";
        public override ConversionType conversionType => ConversionType.Normal;

        public override object OnSelection(StructComponent<Vector2> input) => input;
    }
}
