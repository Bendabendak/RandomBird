using SadJam;
using SadJam.Components;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SadJamEditor.Components
{
    [CustomStructConvertor(typeof(StructComponent<Vector3>))]
    public class Convertor_QuaternionToVector3 : StructConvertor<StructComponent<Quaternion>>
    {
        public override void ConvertMeAs(GameObject target, Type targetType, StructComponent<Quaternion> input, object before, Action<object> done, params object[] customData)
        {
            SplitSelection(input, 3, (string path, List<object> result) =>
            {
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
