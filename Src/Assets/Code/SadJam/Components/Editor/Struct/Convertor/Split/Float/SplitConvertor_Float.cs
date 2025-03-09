using SadJam;

namespace SadJamEditor.Components
{
    public class SplitConvertor_Float : ConversionType<StructComponent<float>>
    {
        public override ConversionType conversionType => ConversionType.Split;
        public override string uniqueAddress => "Value";

        public override object OnSelection(StructComponent<float> input) => input;
    }
}
