using SadJam;
using System;
using UnityEngine;

namespace SadJamEditor.Components
{
    [CustomStructConvertor(typeof(StringComponent))]
    public class Convertor_FloatToString : StructConvertor<StructComponent<float>>
    {
        public override void ConvertMeAs(GameObject target, Type targetType, StructComponent<float> input, object before, Action<object> done, params object[] customData)
        {
            done(Convertor_ToString.Convert(target, input, before));
        }
    }
}
