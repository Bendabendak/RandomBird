using System.Collections.Generic;
using UnityEngine;

namespace SadJam.Components
{
    public class SizeConvertor_FloatToVector3 : SizeConvertor<Vector3>
    {
        public override Vector3 ConvertSize(List<UnityEngine.Component> inputs, string[] customData)
          => new (GetValue(inputs[0]), GetValue(inputs[1]), GetValue(inputs[2]));
        
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
