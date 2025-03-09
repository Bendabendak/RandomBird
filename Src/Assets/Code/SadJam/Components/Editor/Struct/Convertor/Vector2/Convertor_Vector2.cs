using System;
using System.Collections.Generic;
using System.Linq;
using SadJam;
using SadJam.Components;
using UnityEngine;

namespace SadJamEditor.Components
{
    [CustomStructConvertor(typeof(StructComponent<Vector2>))]
    public class Convertor_Vector2 : StructConvertor<StructComponent<Vector2>>
    {
        public override void ConvertMeAs(GameObject target, Type targetType, StructComponent<Vector2> input, object before, Action<object> done, params object[] customData)
        {
            ConversionTypeSelection(input, 2, (string path, List<object> result) =>
            {
                StructComponent<Vector2> v2Adapter = (StructComponent<Vector2>)result.FirstOrDefault(o => o as StructComponent<Vector2>);

                if (v2Adapter != null)
                {
                    done(v2Adapter);
                    return;
                }

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
