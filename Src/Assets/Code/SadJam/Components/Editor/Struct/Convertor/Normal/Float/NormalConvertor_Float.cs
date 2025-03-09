using SadJam;

namespace SadJamEditor.Components
{
    public class NormalConvertor_Float : ConversionType<StructComponent<float>>
    {
        public override string uniqueAddress => "Normal";
        public override ConversionType conversionType => ConversionType.Normal;

        public override object OnSelection(StructComponent<float> input) => input;
    }
}
