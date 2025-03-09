using SadJam;
using System;
using UnityEngine;

namespace SadJamEditor.Components
{
    [CustomStructConvertor(typeof(StringComponent))]
    public class Convertor_Vector3ToString : StructConvertor<StructComponent<Vector3>>
    {
        public override void ConvertMeAs(GameObject target, Type targetType, StructComponent<Vector3> input, object before, Action<object> done, params object[] customData)
        {
            done(Convertor_ToString.Convert(target, input, before));
        }
    }
}
