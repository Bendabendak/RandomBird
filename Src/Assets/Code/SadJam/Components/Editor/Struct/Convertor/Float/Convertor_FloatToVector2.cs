using System;
using System.Collections.Generic;
using SadJam;
using SadJam.Components;
using UnityEngine;

namespace SadJamEditor.Components
{
    [CustomStructConvertor(typeof(StructComponent<Vector2>))]
    public class Convertor_FloatToVector2 : StructConvertor<StructComponent<float>>
    {
        public override void ConvertMeAs(GameObject target, Type targetType, StructComponent<float> input, object before, Action<object> done, params object[] customData)
        {
            SplitSelection(input, 2, (string path, List<object> result) =>
            {
                List<UnityEngine.Component> newInputs = new(new UnityEngine.Component[2]);
                if (before is StructAdapterComponent<Vector2> adapter)
                {
                    newInputs.AddRange(0, adapter.GetInputs<StructComponent<float>>());
                }

                int pos = -1;
                foreach (object r in result)
                {
                    pos++;
                    if (r == null) continue;

                    newInputs[pos] = (UnityEngine.Component)r;
                }

                Adapter_Vector2 newAdapter = Adapter_Vector2.GetAdapter<Adapter_Vector2, SizeConvertor_FloatToVector2>(target, newInputs);

                done(newAdapter);
            });
        }
    }
}
