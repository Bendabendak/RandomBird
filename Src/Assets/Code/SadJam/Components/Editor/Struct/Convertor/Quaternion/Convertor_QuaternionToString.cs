using SadJam;
using System;
using UnityEngine;

namespace SadJamEditor.Components
{
    [CustomStructConvertor(typeof(StringComponent))]
    public class Convertor_QuaternionToString : StructConvertor<StructComponent<Quaternion>>
    {
        public override void ConvertMeAs(GameObject target, Type targetType, StructComponent<Quaternion> input, object before, Action<object> done, params object[] customData)
        {
            done(Convertor_ToString.Convert(target, input, before));
        }
    }
}
