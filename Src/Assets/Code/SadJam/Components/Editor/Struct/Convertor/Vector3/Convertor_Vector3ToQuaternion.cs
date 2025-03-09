using SadJam;
using SadJam.Components;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SadJamEditor.Components
{
    [CustomStructConvertor(typeof(StructComponent<Quaternion>))]
    public class Convertor_Vector3ToQuaternion : StructConvertor<StructComponent<Vector3>>
    {
        public override void ConvertMeAs(GameObject target, Type targetType, StructComponent<Vector3> input, object before, Action<object> done, params object[] customData)
        {
            SplitSelection(input, 4, (string path, List<object> result) =>
            {
                List<UnityEngine.Component> newInputs = new(new UnityEngine.Component[4]);

                if (before is StructAdapterComponent<Quaternion> adapter)
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

                Adapter_Quaternion newAdapter = Adapter_Quaternion.GetAdapter<Adapter_Quaternion, SizeConvertor_FloatToQuaternion>(target, newInputs);

                done(newAdapter);
            });
        }
    }
}
