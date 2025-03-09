using UnityEngine;

namespace SadJam.Components
{
    public class Input_MousePos : StructComponent<Vector3>
    {
        public override Vector3 Size => Input.mousePosition;
    }
}
