using System.Collections.Generic;
using UnityEngine;

namespace SadJam.Components
{
    public class SizeConvertor_FloatToQuaternion : SizeConvertor<Quaternion>
    {
        public override Quaternion ConvertSize(List<UnityEngine.Component> inputs, string[] customData)
              => new(GetValue(inputs[0]), GetValue(inputs[1]), GetValue(inputs[2]), GetValue(inputs[3]));

        private float GetValue(UnityEngine.Component input)
        {
            if (input is StructComponent<float> inputStruct)
            {
                return inputStruct.Size;
            }

            return float.NaN;
        }
    }
}
