using SadJam;
using UnityEngine;

namespace SadJamEditor.Components
{
    public class NormalConvertor_Quaternion : ConversionType<StructComponent<Quaternion>>
    {
        public override string uniqueAddress => "Normal";
        public override ConversionType conversionType => ConversionType.Normal;

        public override object OnSelection(StructComponent<Quaternion> input) => input;
    }
}
