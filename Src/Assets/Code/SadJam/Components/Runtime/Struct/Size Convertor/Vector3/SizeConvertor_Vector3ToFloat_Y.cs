using System.Collections.Generic;
using UnityEngine;

namespace SadJam.Components
{
    public class SizeConvertor_Vector3ToFloat_Y : SizeConvertor<float>
    {
        public override float ConvertSize(List<UnityEngine.Component> inputs, string[] customData)
            => ((StructComponent<Vector3>)inputs[0]).Size.y;
    }
}