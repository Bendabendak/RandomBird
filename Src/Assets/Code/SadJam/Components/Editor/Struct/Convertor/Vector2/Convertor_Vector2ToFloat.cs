using SadJam;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SadJamEditor.Components
{
    [CustomStructConvertor(typeof(StructComponent<float>))]
    public class Convertor_Vector2ToFloat : StructConvertor<StructComponent<Vector2>>
    {
        public override void ConvertMeAs(GameObject target, Type targetType, StructComponent<Vector2> input, object before, Action<object> done, params object[] customData)
        {
            SplitSelection(input, 1, (string path, List<object> result) =>
            {
                done((UnityEngine.Component)result[0]);
            });
        }
    }
}
