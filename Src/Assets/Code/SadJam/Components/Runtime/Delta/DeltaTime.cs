using UnityEngine;

namespace SadJam.Components
{
    [StaticStructComponent]
    public class DeltaTime : StructComponent<float>
    {
        public override float Size => Time.deltaTime;
    }
}
