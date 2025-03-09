using System;
using System.Collections.Generic;
using UnityEngine;
using SadJam;
using SadJam.Components;

namespace SadJamEditor.Components
{
    [CustomStructConvertor(typeof(StructComponent<Vector2>))]
    public class Convertor_Vector3ToVector2 : StructConvertor<StructComponent<Vector3>>
    {
        public override void ConvertMeAs(GameObject target, Type targetType, StructComponent<Vector3> input, object before, Action<object> done, params object[] customData)
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
