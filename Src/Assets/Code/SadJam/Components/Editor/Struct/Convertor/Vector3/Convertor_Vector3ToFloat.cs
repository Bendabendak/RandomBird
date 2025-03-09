using System;
using System.Collections.Generic;
using SadJam;
using UnityEngine;

namespace SadJamEditor.Components
{
    [CustomStructConvertor(typeof(StructComponent<float>))]
    public class Convertor_Vector3ToFloat : StructConvertor<StructComponent<Vector3>>
    {
        public override void ConvertMeAs(GameObject target, Type targetType, StructComponent<Vector3> input, object before, Action<object> done, params object[] customData)
        {
            SplitSelection(input, 1, (string path, List<object> result) =>
            {
                done((UnityEngine.Component)result[0]);
            });
        }
    }
}
