using SadJam;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SadJamEditor.Components
{
    [CustomStructConvertor(typeof(StructComponent<float>))]
    public class Convertor_QuaternionToFloat : StructConvertor<StructComponent<Quaternion>>
    {
        public override void ConvertMeAs(GameObject target, Type targetType, StructComponent<Quaternion> input, object before, Action<object> done, params object[] customData)
        {
            SplitSelection(input, 1, (string path, List<object> result) =>
            {
                done((UnityEngine.Component)result[0]);
            });
        }
    }
}
