using System.Collections.Generic;
using UnityEngine;

namespace SadJam.Components
{
    public class SizeConvertor_QuaternionToFloat_Y : SizeConvertor<float>
    {
        public override float ConvertSize(List<UnityEngine.Component> inputs, string[] customData)
            => ((StructComponent<Quaternion>)inputs[0]).Size.y;
    }
}
