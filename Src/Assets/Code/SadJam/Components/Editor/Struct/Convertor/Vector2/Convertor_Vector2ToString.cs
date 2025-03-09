using SadJam;
using System;
using UnityEngine;

namespace SadJamEditor.Components
{
    [CustomStructConvertor(typeof(StringComponent))]
    public class Convertor_Vector2ToString : StructConvertor<StructComponent<Vector2>>
    {
        public override void ConvertMeAs(GameObject target, Type targetType, StructComponent<Vector2> input, object before, Action<object> done, params object[] customData)
        {
            done(Convertor_ToString.Convert(target, input, before));
        }
    }
}
