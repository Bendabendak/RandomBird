using System;
using System.Collections.Generic;
using SadJam;
using UnityEngine;

namespace SadJamEditor.Components
{
    [CustomStructConvertor(typeof(StructComponent<float>))]
    public class Convertor_Float : StructConvertor<StructComponent<float>>
    {
        public override void ConvertMeAs(GameObject target, Type targetType, StructComponent<float> input, object before, Action<object> done, params object[] customData)
        {
            NormalSelection(input, (string path, List<object> result) =>
            {
                done((UnityEngine.Component)result[0]);
            });
        }
    }
}
