using System;
using System.Collections.Generic;
using System.Linq;
using SadJam;
using SadJam.Components;
using UnityEngine;

namespace SadJamEditor.Components
{
    [CustomStructConvertor(typeof(StructComponent<Vector3>))]
    public class Convertor_Vector3 : StructConvertor<StructComponent<Vector3>>
    {
        public override void ConvertMeAs(GameObject target, Type targetType, StructComponent<Vector3> input, object before, Action<object> done, params object[] customData)
        {
            ConversionTypeSelection(input, 3, (string path, List<object> result) =>
            {
                StructComponent<Vector3> v3Adapter = (StructComponent<Vector3>)result.FirstOrDefault(o => o as StructComponent<Vector3>);

                if (v3Adapter != null)
                {
                    done(v3Adapter);
                    return;
                }

                List<UnityEngine.Component> newInputs = new(new UnityEngine.Component[3]);
                if (before is StructAdapterComponent<Vector3> adapter)
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

                Adapter_Vector3 newAdapter = Adapter_Vector3.GetAdapter<Adapter_Vector3, SizeConvertor_FloatToVector3>(target, newInputs);

                done(newAdapter);
            });
        }
    }
}
